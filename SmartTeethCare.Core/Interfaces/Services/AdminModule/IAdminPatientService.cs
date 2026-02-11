using SmartTeethCare.Core.DTOs.AdminModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Services.AdminModule
{
    public interface IAdminPatientService
    {
        Task<string> CreatePatientAsync(CreatePatientByAdminDTO dto);
    }
}
