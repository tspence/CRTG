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
using CRTG.Common.Data;

namespace CRTG.Sensors.SensorLibrary
{
    [SensorUI(Category = "WMI", Tooltip = "Measure disk activity via WMI.")]
    public class WmiDiskActivity : BaseSensor
    {
        #region Implementation

        public override CollectResult Collect()
        {
            var coll = WmiHelper.WmiQuery(Device, "SELECT * FROM Win32_PerfFormattedData_PerfOS_System");

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
                return new CollectResult(total / count);
            }

            // Failed!
            throw new Exception("No disk query found!");
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
