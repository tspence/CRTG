using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace CRTG.Sensors.SensorLibrary
{
    public class WmiDiskActivity : BaseSensor
    {
        public override decimal Collect()
        {
            var coll = Device.WmiQuery("SELECT * FROM Win32_PerfFormattedData_PerfOS_System");

            // Count usage of each OS instance (should really be only one!)
            decimal total = 0;
            int count = 0;
            foreach (ManagementObject queryObj in coll) {
                object o = queryObj["FileDataOperationsPersec"];
                count++;
                total += (decimal)Convert.ChangeType(o, typeof(decimal));
            }

            // Average them out
            if (count > 0) {
                return total / count;
            }

            // Failed!
            throw new Exception("No disk query found!");
        }
    }
}
