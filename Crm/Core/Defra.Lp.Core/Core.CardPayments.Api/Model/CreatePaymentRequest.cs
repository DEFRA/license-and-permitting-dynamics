using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace CardPayments.Model
{
    public class CreatePaymentRequest
    {
        public int amount { get; set; }

        public string reference { get; set; }

        public string return_url { get; set; }

        public string description { get; set; }
    }
}
