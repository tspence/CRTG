using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Common
{
    public class SensorDataCollection
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<SensorData> Data { get; set; }
        public List<SensorException> Exceptions { get; set; }
    }
}
