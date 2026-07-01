using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SmartTeethCare.Core.DTOs.DoctorModule;
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
        // STEP 1: المريض يختار موعد → نشوف Reservation → نعمل PaymentIntent
        // ============================================================
        public async Task<PaymentResponse> CreatePaymentIntent(CreatePaymentRequest request, int patientId)
        {
            var reservationRepo = _unitOfWork.Repository<SlotReservation>();

            // ✅ في حد تاني حاجز نفس الـ Slot ومش expired؟
            var existing = await reservationRepo.FindAsync(r =>
                r.DoctorId == request.DoctorId &&
                r.Date.Date == request.Date.Date &&
                r.StartTime == request.StartTime &&
                r.ExpiresAt > DateTime.UtcNow);

            if (existing.Any())
                throw new InvalidOperationException(
                    "هذا الموعد محجوز مؤقتاً، يرجى الانتظار 10 دقائق أو اختيار موعد آخر");

            // ✅ عمل PaymentIntent على Stripe
            var options = new PaymentIntentCreateOptions
            {
                Amount = 50 * 100, // 50 EGP deposit
                Currency = "egp",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                    AllowRedirects = "never",
                },
                // بنحط بيانات الموعد في Metadata عشان الـ Webhook يحجز بعدين
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

            // ✅ عمل Reservation لـ 10 دقايق
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
        // STEP 2: Stripe بتبعت Webhook بعد الدفع
        // ============================================================
        public async Task HandleWebhook(string json, string stripeSignature)
        {
            var webhookSecret = _config["Stripe:WebhookSecret"];

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    webhookSecret,
                    throwOnApiVersionMismatch: false);
            }
            catch (StripeException ex)
            {
                throw new Exception(ex.Message);
            }

            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent != null)
                    await HandlePaymentSuccess(paymentIntent);
            }
        }

        // ============================================================
        // STEP 3: الدفع نجح → نحجز + نمسح Reservation + Notification
        // ============================================================
        private async Task HandlePaymentSuccess(PaymentIntent paymentIntent)
        {
            // 1. Get latest PaymentIntent from Stripe
            var paymentIntentService = new PaymentIntentService();

            var latestPaymentIntent =
                await paymentIntentService.GetAsync(paymentIntent.Id);

            // 2. Verify payment status
            if (latestPaymentIntent.Status != "succeeded")
                return;

            // 3. Check if already processed
            var existingAppointment = (await _unitOfWork.Repository<Appointment>()
                .FindAsync(a => a.PaymentIntentId == paymentIntent.Id))
                .FirstOrDefault();

            if (existingAppointment != null)
                return;

            // 4. Check reservation exists
            var reservation = (await _unitOfWork.Repository<SlotReservation>()
                .FindAsync(r => r.PaymentIntentId == paymentIntent.Id))
                .FirstOrDefault();

            if (reservation == null)
                throw new Exception("Reservation not found.");


            var meta = latestPaymentIntent.Metadata;

            // ✅ لو Metadata فاضية = test event من stripe trigger، ignore
            if (meta == null || !meta.ContainsKey("doctorId"))
            {
                Console.WriteLine("⚠️ Test event - no metadata, skipping");
                return;
            }

            var dto = new BookAppointmentDto
            {
                DoctorId = int.Parse(meta["doctorId"]),
                PatientId = int.Parse(meta["patientId"]),
                Date = DateTime.Parse(meta["date"]),
                StartTime = TimeSpan.Parse(meta["startTime"]),
                PaymentMethod = meta["paymentMethod"],
                CreatedByAdmin = false,
                PaymentIntentId = latestPaymentIntent.Id
            };


            Console.WriteLine($"📌 Booking: DoctorId={dto.DoctorId}, PatientId={dto.PatientId}, Date={dto.Date}, Start={dto.StartTime}");

            var result = await _bookingService.BookAppointmentAsync(dto);

            Console.WriteLine($"📌 Booking Result: Success={result.Success}, Message={result.Message}");

            if (!result.Success)
            {
                await _unitOfWork.Repository<SlotReservation>()
                    .DeleteAsync(reservation);

                await _unitOfWork.CompleteAsync();

                Console.WriteLine(result.Message);
                return;
            }

            // ✅ حجز الموعد
            //var result = await _bookingService.BookAppointmentAsync(dto);
            //if (!result.Success) return;

            var appointment = await _unitOfWork.Repository<Appointment>()
                .GetByIdAsync(result.AppointmentId!.Value);
            if (appointment == null) return;

            // ✅ PaymentStatus = Paid
            appointment.PaymentStatus = AppointmentPaymentStatus.Paid;
            appointment.PaymentMethod = Enum.Parse<AppointmentPaymentMethod>(
                    meta["paymentMethod"],
                    true);
            await _unitOfWork.Repository<Appointment>().UpdateAsync(appointment);

            // ✅ امسح الـ Reservation
            var reservations = await _unitOfWork.Repository<SlotReservation>()
                .FindAsync(r => r.PaymentIntentId == paymentIntent.Id);
            reservation = reservations.FirstOrDefault();
            if (reservation != null)
                await _unitOfWork.Repository<SlotReservation>().DeleteAsync(reservation);

            await _unitOfWork.CompleteAsync();

            // ✅ جيب الـ Patient واسم الدكتور للـ Notification
            var patients = await _unitOfWork.Repository<Patient>()
                .FindAsync(p => p.Id == dto.PatientId);
            var patient = patients.FirstOrDefault();
            if (patient == null) return;

            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(dto.DoctorId);
            var doctorUser = doctor != null ? await _userManager.FindByIdAsync(doctor.UserId) : null;
            var doctorName = doctorUser?.DisplayName ?? doctorUser?.UserName ?? "Doctor";

            try
            {
                // 📩 Notification: اتحجز بنجاح
                await _notificationService.CreateAsync(
                    patient.UserId,
                    "Appointment Confirmed",
                    "Your appointment has been confirmed successfully.",
                    true,
                    new Dictionary<string, string>
                    {
                        { "DATE", appointment.Date.ToString("dd/MM/yyyy") + " " + DateTime.Today.Add(appointment.StartTime).ToString("hh:mm tt") },
                        { "DOCTOR", doctorName }
                    }
                );


                // ⏰ Reminder قبل الموعد بـ 3 ساعات
                var appointmentDateTime = appointment.Date.Date + appointment.StartTime;
                var reminderTime = appointmentDateTime.ToUniversalTime().AddHours(-3);
                if (reminderTime > DateTime.UtcNow)
                {
                    BackgroundJob.Schedule<INotificationService>(
                        x => x.SendReminderAsync(appointment.Id),
                        reminderTime);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception: {ex.Message}");
                Console.WriteLine($"❌ Inner: {ex.InnerException?.Message}");
            }
        }
    }
}