using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Enums;
using SmartTeethCare.Core.Interfaces.Services.PatientModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using System.Security.Claims;


namespace SmartTeethCare.Service.PatientModule
{
    public class PatientReviewService : IPatientReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddReviewAsync(AddReviewDTO dto, ClaimsPrincipal user)
        {
            // 1️⃣ Get PatientId from logged user
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not authenticated");

            var patients = await _unitOfWork.Repository<Patient>()
                .FindAsync(p => p.UserId == userId);

            var patient = patients.FirstOrDefault();

            if (patient == null)
                throw new Exception("Patient not found");

            var patientId = patient.Id;


            // 2️⃣ check appointment
            var appointment = (await _unitOfWork.Repository<Appointment>()
                .FindAsync(a =>
                    a.Id == dto.AppointmentId &&
                    a.PatientID == patientId &&
                    a.DoctorID == dto.DoctorId &&
                    a.Status == AppointmentStatus.Completed
                )).FirstOrDefault();

            if (appointment == null)
                throw new Exception("You can only review a completed appointment");


            // 3️⃣ prevent duplicate review
            var alreadyReviewed = await _unitOfWork.Repository<Review>()
                .AnyAsync(r => r.AppointmentID == dto.AppointmentId);

            if (alreadyReviewed)
                throw new Exception("You already reviewed this appointment");


            // 4️⃣ validate rating
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new Exception("Rating must be between 1 and 5");


            // 5️⃣ create review
            var review = new Review
            {
                DentistID = dto.DoctorId,
                PatientID = patientId,
                AppointmentID = dto.AppointmentId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Review>().AddAsync(review);
        }
        public async Task UpdateReviewAsync(int reviewId, UpdateReviewDTO dto, ClaimsPrincipal user)
        {
            // 1️⃣ get patientId
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not authenticated");

            var patients = await _unitOfWork.Repository<Patient>()
                .FindAsync(p => p.UserId == userId);

            var patient = patients.FirstOrDefault();

            if (patient == null)
                throw new Exception("Patient not found");

            var patientId = patient.Id;


            // 2️⃣ get review
            var review = await _unitOfWork.Repository<Review>().GetByIdAsync(reviewId);

            if (review == null || review.PatientID != patientId)
                throw new Exception("Review not found");


            // 3️⃣ 24 hour edit limit
            if ((DateTime.UtcNow - review.CreatedAt).TotalHours > 24)
                throw new Exception("You can edit review only within 24 hours");


            // 4️⃣ validate rating
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new Exception("Rating must be between 1 and 5");


            // 5️⃣ update
            review.Comment = dto.Comment;
            review.Rating = dto.Rating;
            review.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Review>().UpdateAsync(review);
        }

        public async Task DeleteReviewAsync(int reviewId, ClaimsPrincipal user)
        {
            // 1️⃣ get patientId
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not authenticated");

            var patients = await _unitOfWork.Repository<Patient>()
                .FindAsync(p => p.UserId == userId);

            var patient = patients.FirstOrDefault();

            if (patient == null)
                throw new Exception("Patient not found");

            var patientId = patient.Id;


            // 2️⃣ get review
            var review = await _unitOfWork.Repository<Review>().GetByIdAsync(reviewId);

            if (review == null || review.PatientID != patientId)
                throw new Exception("Review not found");


            await _unitOfWork.Repository<Review>().DeleteAsync(review);
        }
        public async Task<IEnumerable<ReviewViewDTO>> GetMyReviewsAsync(ClaimsPrincipal user)
        {
            // 1️⃣ get patientId
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)?? throw new Exception("User not authenticated");

            var patients = await _unitOfWork.Repository<Patient>()
                .FindAsync(p => p.UserId == userId);

            var patient = patients.FirstOrDefault();

            if (patient == null)
                throw new Exception("Patient not found");

            var patientId = patient.Id;


            // 2️⃣ get reviews
            var reviews = await _unitOfWork.Repository<Review>()
                .FindAsync(r => r.PatientID == patientId,
                    q => q.Include(r => r.doctor).ThenInclude(d => d.User));


            return reviews.Select(r => new ReviewViewDTO
            {
                Id = r.Id,
                DoctorId = r.DentistID,
                DoctorName = r.doctor?.User?.UserName ?? "Unknown Doctor",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            });
        }
        public async Task<IEnumerable<ReviewViewDTO>> GetMyReviewsForDoctorAsync(int doctorId, ClaimsPrincipal user)
        {
            // 1️⃣ get patientId
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)?? throw new Exception("User not authenticated");

            var patients = await _unitOfWork.Repository<Patient>()
                .FindAsync(p => p.UserId == userId);

            var patient = patients.FirstOrDefault();

            if (patient == null)
                throw new Exception("Patient not found");

            var patientId = patient.Id;


            // 2️⃣ get reviews
            var reviews = await _unitOfWork.Repository<Review>()
                .FindAsync(r => r.PatientID == patientId && r.DentistID == doctorId,
                    q => q.Include(r => r.doctor).ThenInclude(d => d.User));


            return reviews.Select(r => new ReviewViewDTO
            {
                Id = r.Id,
                DoctorId = r.DentistID,
                DoctorName = r.doctor?.User?.UserName ?? "Unknown Doctor",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            });
        }





    }
}
