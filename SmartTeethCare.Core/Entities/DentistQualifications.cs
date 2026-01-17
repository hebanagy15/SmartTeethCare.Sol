using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Entities
{
    public class DentistQualifications : BaseEntity   
    {
       

        public Doctor doctor { get; set; }
        public int DoctorId { get; set; }



    }
}
