using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartTeethCare.Core.Entities
{
    public class Admin : BaseEntity
    {
      

        public string Role { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }



    }
}
