using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Lp.TestSupport.Mock
{
    public class MockTracingService : ITracingService
    {
        public void Trace(string format, params object[] args)
        { 
        }
    }
}
