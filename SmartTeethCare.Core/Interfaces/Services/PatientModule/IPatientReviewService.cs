using SmartTeethCare.Core.DTOs.PatientModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.PatientModule
{
    public interface IPatientReviewService
    {
        Task AddReviewAsync(AddReviewDTO dto, ClaimsPrincipal user);
        Task UpdateReviewAsync(int reviewId, UpdateReviewDTO dto, ClaimsPrincipal user);
        Task DeleteReviewAsync(int reviewId, ClaimsPrincipal user);
        Task<IEnumerable<ReviewViewDTO>> GetMyReviewsAsync(ClaimsPrincipal user);
        Task<IEnumerable<ReviewViewDTO>> GetMyReviewsForDoctorAsync(int doctorId, ClaimsPrincipal user);
    }
}
