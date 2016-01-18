using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Common
{
    public class SensorException
    {
        public DateTime ExceptionTime { get; set; }
        public string Description { get; set; }
        public Exception Item { get; set; }
        public bool Cleared { get; set; }
    }
}
