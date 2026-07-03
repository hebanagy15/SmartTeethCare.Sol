using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SmartTeethCare.Core.DTOs.DoctorModule;
using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.DTOs.Payment;
using SmartTeethCare.Core.DTOs.Stripe;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Enums;
using SmartTeethCare.Core.Interfaces.Services.DoctorModule;
using SmartTeethCare.Core.Interfaces.Services.NotificationService;
using SmartTeethCare.Core.Interfaces.Services.Stripe;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using Stripe;

namespace SmartTeethCare.Service.Services.Stripe
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly INotificationService _notificationService;
        private readonly UserManager<User> _userManager;
        private readonly IAppointmentBookingService _bookingService;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IConfiguration config,
            INotificationService notificationService,
            UserManager<User> userManager,
            IAppointmentBookingService bookingService)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _notificationService = notificationService;
            _userManager = userManager;
            _bookingService = bookingService;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        // ============================================================
        // STEP 1: المريض يختار موعد → PaymentIntent + Reservation
        // ============================================================
        public async Task<PaymentResponse> CreatePaymentIntent(CreatePaymentRequest request, int patientId)
        {
            var reservationRepo = _unitOfWork.Repository<SlotReservation>();

            // 1. التأكد أن الموعد في المستقبل
            if (request.Date.Date < DateTime.Today)
                throw new InvalidOperationException("لا يمكن حجز موعد في الماضي");

            if (request.Date.Date == DateTime.Today && request.StartTime < DateTime.Now.TimeOfDay)
                throw new InvalidOperationException("لا يمكن حجز موعد في الماضي");

            // 2. التأكد أن الدكتور شغال اليوم ده
            var dayOfWeek = request.Date.DayOfWeek;
            var schedules = await _unitOfWork.Repository<DoctorSchedule>()
                .FindAsync(s => s.DoctorId == request.DoctorId && s.DayOfWeek == dayOfWeek);
            var schedule = schedules.FirstOrDefault();
            if (schedule == null)
                throw new InvalidOperationException("الدكتور غير متاح في هذا اليوم");

            // 3. التأكد أن الوقت جوه ساعات العمل
            var endTime = request.StartTime + TimeSpan.FromMinutes(schedule.SlotDurationMinutes);
            if (request.StartTime < schedule.StartTime || endTime > schedule.EndTime)
                throw new InvalidOperationException("الوقت المختار خارج ساعات عمل الدكتور");

            // 4. التأكد أن الموعد غير محجوز ومؤكد بالفعل في جدول المواعيد
            var booked = await _unitOfWork.Repository<Appointment>().FindAsync(a =>
                a.DoctorID == request.DoctorId &&
                a.Date.Date == request.Date.Date &&
                a.StartTime == request.StartTime &&
                a.Status != AppointmentStatus.Rejected);

            if (booked.Any())
                throw new InvalidOperationException("هذا الموعد محجوز بالفعل، يرجى اختيار موعد آخر");

            // 5. التأكد أن الموعد ليس محجوزاً مؤقتاً بواسطة شخص آخر يدفع الآن
            var existing = await reservationRepo.FindAsync(r =>
                r.DoctorId == request.DoctorId &&
                r.Date.Date == request.Date.Date &&
                r.StartTime == request.StartTime &&
                r.ExpiresAt > DateTime.UtcNow);

            if (existing.Any())
                throw new InvalidOperationException(
                    "هذا الموعد محجوز مؤقتاً، يرجى الانتظار 10 دقائق أو اختيار موعد آخر");

            var options = new PaymentIntentCreateOptions
            {
                Amount = 50 * 100,
                Currency = "egp",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                    AllowRedirects = "never"
                },
                Metadata = new Dictionary<string, string>
                {
                    { "doctorId",      request.DoctorId.ToString() },
                    { "patientId",     patientId.ToString() },
                    { "date",          request.Date.ToString("o") },
                    { "startTime",     request.StartTime.ToString() },
                    { "paymentMethod", request.PaymentMethod }
                }
            };

            var stripeService = new PaymentIntentService();
            var paymentIntent = await stripeService.CreateAsync(options);

            await reservationRepo.AddAsync(new SlotReservation
            {
                DoctorId = request.DoctorId,
                PatientId = patientId,
                Date = request.Date.Date,
                StartTime = request.StartTime,
                PaymentIntentId = paymentIntent.Id,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            });

            await _unitOfWork.CompleteAsync();

            return new PaymentResponse
            {
                ClientSecret = paymentIntent.ClientSecret,
                PaymentIntentId = paymentIntent.Id
            };
        }

        // ============================================================
        // STEP 2: Webhook من Stripe (Source of Truth الرسمي)
        // ============================================================
        public async Task HandleWebhook(string json, string stripeSignature)
        {
            var webhookSecret = _config["Stripe:WebhookSecret"];

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, webhookSecret);
            }
            catch (StripeException ex)
            {
                throw new Exception($"Invalid Stripe signature: {ex.Message}");
            }

            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent != null)
                    await ProcessSuccessfulPayment(paymentIntent);
            }
            // ✅ لو دفع فاشل (كارت غلط) → مد وقت الحجز المؤقت 10 دقائق تانية عشان يحاول تاني
            else if (stripeEvent.Type == "payment_intent.payment_failed")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent != null)
                {
                    var reservationRepo = _unitOfWork.Repository<SlotReservation>();
                    var reservations = await reservationRepo
                        .FindAsync(r => r.PaymentIntentId == paymentIntent.Id);
                    var reservation = reservations.FirstOrDefault();
                    if (reservation != null)
                    {
                        // مد الوقت 10 دقائق من دلوقتي عشان المريض يقدر يحاول بكارت تاني
                        reservation.ExpiresAt = DateTime.UtcNow.AddMinutes(10);
                        await reservationRepo.UpdateAsync(reservation);
                        await _unitOfWork.CompleteAsync();
                    }
                }
            }
        }

        // ============================================================
        // STEP 3 (جديد): Confirm Endpoint اللي الفرونت بينده عليه
        // بعد ما confirmCardPayment ينجح
        // ============================================================
        public async Task<AppointmentDetailsDTO> ConfirmPayment(string paymentIntentId, int patientId)
        {
            var stripeService = new PaymentIntentService();
            var paymentIntent = await stripeService.GetAsync(paymentIntentId);

            if (paymentIntent == null)
                throw new Exception("Payment not found");

            // ✅ متصدقش الفرونت، اتأكد من Stripe نفسه
            if (paymentIntent.Status != "succeeded")
                throw new InvalidOperationException("لم يتم إتمام الدفع بعد");

            // ✅ اتأكد إن الـ PaymentIntent ده فعلاً بتاع المريض ده
            if (!paymentIntent.Metadata.TryGetValue("patientId", out var metaPatientId) ||
                metaPatientId != patientId.ToString())
                throw new UnauthorizedAccessException("هذه العملية لا تخصك");

            var appointment = await ProcessSuccessfulPayment(paymentIntent);

            if (appointment == null)
                throw new Exception("تعذر تأكيد الحجز، يرجى التواصل مع الدعم الفني");

            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(appointment.DoctorID);
            var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(appointment.PatientID);
            var doctorUser = doctor != null ? await _userManager.FindByIdAsync(doctor.UserId) : null;
            var patientUser = patient != null ? await _userManager.FindByIdAsync(patient.UserId) : null;

            return new AppointmentDetailsDTO
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorID,
                DoctorName = doctorUser?.UserName ?? "Unknown",
                PatientId = appointment.PatientID,
                PatientName = patientUser?.UserName ?? "Unknown",
                Amount = appointment.Amount,
                Date = appointment.Date,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status.ToString(),
                CreatedAt = appointment.CreatedAt
            };
        }

        // ============================================================
        // المنطق المشترك (Idempotent) بين الـ Webhook والـ Confirm
        // ============================================================
        private async Task<Appointment?> ProcessSuccessfulPayment(PaymentIntent paymentIntent)
        {
            var reservationRepo = _unitOfWork.Repository<SlotReservation>();

            var reservations = await reservationRepo
                .FindAsync(r => r.PaymentIntentId == paymentIntent.Id);
            var reservation = reservations.FirstOrDefault();

            // ✅ لو مفيش Reservation → يبقى واحد تاني (Webhook أو Confirm) سبق وعالجها
            // نرجع الـ Appointment الموجود بالفعل (Idempotent، مفيش Duplicate)
            if (reservation == null)
            {
                var existing = await _unitOfWork.Repository<Appointment>()
                    .FindAsync(a => a.PaymentIntentId == paymentIntent.Id);
                return existing.FirstOrDefault();
            }

            var meta = paymentIntent.Metadata;

            var dto = new BookAppointmentDto
            {
                DoctorId = int.Parse(meta["doctorId"]),
                PatientId = int.Parse(meta["patientId"]),
                Date = DateTime.Parse(meta["date"]),
                StartTime = TimeSpan.Parse(meta["startTime"]),
                Amount = (int)(paymentIntent.Amount / 100),
                PaymentMethod = meta["paymentMethod"],
                CreatedByAdmin = false,
                PaymentIntentId = paymentIntent.Id
            };

            var result = await _bookingService.BookAppointmentAsync(dto);
            if (!result.Success)
                throw new InvalidOperationException($"Booking failed: {result.Message}");

            var appointment = await _unitOfWork.Repository<Appointment>()
                .GetByIdAsync(result.AppointmentId!.Value);
            if (appointment == null) return null;

            appointment.PaymentStatus = AppointmentPaymentStatus.Paid;
            appointment.PaymentMethod = AppointmentPaymentMethod.Visa;
            await _unitOfWork.Repository<Appointment>().UpdateAsync(appointment);

            await reservationRepo.DeleteAsync(reservation);

            await _unitOfWork.CompleteAsync();

            // ✅ Notification + Reminder (Best Effort)
            try
            {
                var patients = await _unitOfWork.Repository<Patient>()
                    .FindAsync(p => p.Id == dto.PatientId);
                var patient = patients.FirstOrDefault();
                if (patient != null)
                {
                    var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(dto.DoctorId);
                    var doctorUser = doctor != null ? await _userManager.FindByIdAsync(doctor.UserId) : null;
                    var doctorName = doctorUser?.DisplayName ?? doctorUser?.UserName ?? "Doctor";

                    await _notificationService.CreateAsync(
                        patient.UserId,
                        "Appointment Confirmed",
                        "Your appointment has been confirmed successfully.",
                        true,
                        new Dictionary<string, string>
                        {
                            { "DATE",   appointment.Date.ToString("dd/MM/yyyy") + " " + appointment.StartTime },
                            { "DOCTOR", doctorName }
                        }
                    );

                    var appointmentDateTime = appointment.Date.Date + appointment.StartTime;
                    var reminderTime = appointmentDateTime.ToUniversalTime().AddHours(-3);
                    if (reminderTime > DateTime.UtcNow)
                    {
                        BackgroundJob.Schedule<INotificationService>(
                            x => x.SendReminderAsync(appointment.Id),
                            reminderTime);
                    }
                }
            }
            catch { }

            return appointment;
        }
    }
}