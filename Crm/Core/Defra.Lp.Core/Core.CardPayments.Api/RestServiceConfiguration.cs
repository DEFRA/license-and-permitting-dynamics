using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CardPayments
{
    public class RestServiceConfiguration
    {
        public string TargetUrl { get; set; }

        public string TargetHost { get; set; }

        public string ApiKey { get; set; }

        public SecurityProtocolType SecurityProtocol { get; set; }

        public string SecurityHeader { get; set; }
    }
}
