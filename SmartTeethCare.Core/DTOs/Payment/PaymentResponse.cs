using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.DTOs.Payment
{
    public class PaymentResponse
    {
        public string ClientSecret { get; set; }
        public string PaymentIntentId { get; set; }
    }
}
