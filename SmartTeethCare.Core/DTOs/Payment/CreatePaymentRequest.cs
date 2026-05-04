using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.Stripe
{
    public class CreatePaymentRequest
    {
        public int AppointmentId { get; set; }
    }
}
