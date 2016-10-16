/*
 * 2012 - 2016 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/CRTG
 * 
 * This program uses icons from http://www.famfamfam.com/lab/icons/silk/
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using CRTG.Sensors.Toolkit;

namespace CRTG.Sensors.SensorLibrary
{
    [SensorUI(Category = "WMI", Tooltip = "Measure CPU activity via WMI.")]
    public class WmiCpuSensor : BaseSensor
    {
        public override string GetNormalIconPath()
        {
            return "Resources/bricks.png";
        }

        #region Implementation
        public override decimal Collect()
        {
            var coll = WmiHelper.WmiQuery(Device, "SELECT * FROM Win32_PerfFormattedData_Counters_ProcessorInformation");

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
        #endregion
    }
}
