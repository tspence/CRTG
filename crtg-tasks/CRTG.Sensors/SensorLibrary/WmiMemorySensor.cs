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
using CRTG.Common;
using CRTG.Sensors.Toolkit;
using CRTG.Common.Attributes;

namespace CRTG.Sensors.SensorLibrary
{
    [SensorUI(Category = "WMI", Tooltip = "Measure memory statistics via WMI.")]
    public class WmiMemorySensor : BaseSensor
    {
        [AutoUI(Group = "Memory")]
        public MemoryMeasurement Measurement { get; set; }

        #region Implementation

        public override decimal Collect()
        {
            var coll = WmiHelper.WmiQuery(Device, "SELECT * FROM Win32_PerfFormattedData_PerfOS_Memory");

            // Count usage of each OS instance (should really be only one!)
            decimal total = 0;
            int count = 0;
            foreach (ManagementObject queryObj in coll) {
                object o = queryObj["AvailableMBytes"];
                count++;
                total += (decimal)Convert.ChangeType(o, typeof(decimal));
            }

            // Average them out
            if (count > 0) {
                return (total / count);
            }

            // Failed!
            throw new Exception("No OS found!");
        }
        #endregion

        #region Icon
        public override string GetNormalIconPath()
        {
            return "Resources/bricks.png";
        }
        #endregion
    }
}
