using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.DTOs.Lookup;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.Lookup;
using SmartTeethCare.Core.Interfaces.UnitOfWork;


namespace SmartTeethCare.Service.Lookup
{
    public class LookupService : ILookupService
    {
        private readonly IUnitOfWork _unitOfWork;
        public LookupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DoctorDTO>> GetDoctorsAsync()
        {
            var doctors = await _unitOfWork.Repository<Doctor>().FindAsync(
                include: q => q.Include(d => d.User).Include(d => d.Speciality)
            );

            return doctors.Select(d => new DoctorDTO
            {
                Id = d.Id,
                Name = d.User?.UserName?? "Unknown",
                SpecializationId = d.SpecialtyID,
                SpecializationName = d.Speciality?.Name
            });
        }


        public async Task<IEnumerable<SpecializationDTO>> GetSpecializationsAsync()
        {
            var specs = await _unitOfWork.Repository<Speciality>().GetAllAsync();
            return specs.Select(s => new SpecializationDTO { Id = s.Id, Name = s.Name });
        }
    }
}
