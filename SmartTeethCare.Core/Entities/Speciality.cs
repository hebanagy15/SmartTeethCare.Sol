using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Entities
{
    public class Speciality : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<EducationalVideos> EducationalVideos { get; set; }
        public ICollection<Doctor> doctors { get; set; }
    }
}
