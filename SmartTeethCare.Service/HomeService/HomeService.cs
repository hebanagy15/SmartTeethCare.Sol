using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.DTOs.HomeService;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.HomeService;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Service.HomeService
{
    public class HomeService : IHomeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<HomeStatisticsDto> GetStatisticsAsync()
        {
            var patientsCount = await _unitOfWork
                .Repository<Patient>()
                .Query()
                .CountAsync();

            var doctorsCount = await _unitOfWork
                .Repository<Doctor>()
                .Query()
                .CountAsync();

            return new HomeStatisticsDto
            {
                PatientsCount = patientsCount,
                DoctorsCount = doctorsCount
            };
        }

        public async Task<List<TopReviewDto>> GetTopReviewsAsync()
        {
            return await _unitOfWork.Repository<Review>()
                .Query()
                .Include(r => r.Patient)
                .ThenInclude(p => p.User)
                .OrderByDescending(r => r.Rating)
                .ThenByDescending(r => r.CreatedAt) 
                .Take(3)
                .Select(r => new TopReviewDto
                {
                    Rating = r.Rating,
                    Comment = r.Comment,
                    PatientName = r.Patient.User.DisplayName,
                    ProfileImageUrl = r.Patient.ProfileImageUrl
                })
                .ToListAsync();
        }
    }
}
