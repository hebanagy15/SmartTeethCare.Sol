using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Entities
{
    public class EducationalVideos : BaseEntity
    {
        public int SpecialtyID { get; set; }  
       
        public string VideoURL { get; set; }
        public Speciality Speciality { get; set; }  
       
    }
}

