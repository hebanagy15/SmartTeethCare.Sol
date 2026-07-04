using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.DTOs.Pharmacy;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.Pharmacy;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Service.Pharmacy
{
    public class PharmacySearchService : IPharmacySearchService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PharmacySearchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AvailablePharmacyDto>> GetAvailablePharmaciesAsync(int medicineId,double patientLatitude,double patientLongitude)
        {
            var medicines = await _unitOfWork.Repository<PharmacyMedicine>()
    .FindAsync(
        x => x.MedicineID == medicineId && x.StockQuantity > 0,
        query => query.Include(x => x.Pharmacy).Include(x => x.Medicine)
    );

            var result = medicines.Select(x => new AvailablePharmacyDto
            {
                PharmacyId = x.PharmacyID,
                PharmacyName = x.Pharmacy.Name,
                Price = x.Medicine.Price,
                Quantity = x.StockQuantity,

                DistanceInKm = CalculateDistance(
                    patientLatitude,
                    patientLongitude,
                    x.Pharmacy.Latitude,
                    x.Pharmacy.Longitude)
            })
            .OrderBy(x => x.DistanceInKm)
            .ToList();

            return result;
        }

        private double CalculateDistance(double lat1,double lon1,double lat2,double lon2)
        {
            const double R = 6371;

            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) *
                Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) *
                Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return Math.Round(R * c, 2);
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
