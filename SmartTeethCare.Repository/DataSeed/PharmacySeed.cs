using SmartTeethCare.Core.Entities;
using SmartTeethCare.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Repository.DataSeed
{
    public static class PharmacySeed
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (context.Pharmacies.Any())
                return;

            var pharmacies = new List<Pharmacy>
        {
            new Pharmacy
            {
                Name = "El Ezaby Pharmacy",
                Address = "El Galaa Street, Next to Faisal Islamic Bank, Tanta, Gharbia Governorate, Egypt",
                Phone = "01122219915",
                WorkingHours = "Open 24 Hours",
                Latitude = 30.787789051654983, 
                Longitude = 31.000871300747125
            },
            new Pharmacy
            {
                Name = "El Tarshoubi Pharmacy",
                Address = "End of El Estad Street, Opposite 306 Street, Tanta Center, Gharbia Governorate, Egypt",
                Phone = "01016130410",
                WorkingHours = "Open 24 Hours",
                Latitude = 30.818930395850167, 
                Longitude = 30.991967390912617
            },
            new Pharmacy
            {
                Name = "El Estad Pharmacy",
                Address = "El Estad Street, Qism 2 Tanta, Tanta Center, Gharbia Governorate, Egypt",
                Phone = "0403407412",
                WorkingHours = "9:00 AM - 12:00 AM",
                Latitude = 30.810028223561062 ,
                Longitude = 30.994841726919887
            }
        };

            await context.Pharmacies.AddRangeAsync(pharmacies);
            await context.SaveChangesAsync();
        }
    }
}
