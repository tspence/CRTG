using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace CRTG.Sensors.SensorLibrary
{
    public class WmiCpuSensor : BaseSensor
    {
        public override decimal Collect()
        {
            var coll = Device.WmiQuery("SELECT * FROM Win32_PerfFormattedData_Counters_ProcessorInformation");

            // Count usage of each OS instance (should really be only one!)
            decimal total = 0;
            int count = 0;
            foreach (ManagementObject queryObj in coll) {
                object o = queryObj["PercentProcessorTime"];
                count++;
                total += (decimal)Convert.ChangeType(o, typeof(decimal));
            }

            // Average them out
            if (count > 0) {
                return (total / count);
            }

            // Failed!
            throw new Exception("No processors found!");
        }
    }
}
