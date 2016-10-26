/*
 * 2012 - 2016 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/CRTG
 * 
 * This program uses icons from http://www.famfamfam.com/lab/icons/silk/
 */
using System;
using System.Management;
using CRTG.Common;
using CRTG.Sensors.Toolkit;
using CRTG.Common.Attributes;
using CRTG.Common.Data;
using System.Threading.Tasks;

namespace CRTG.Sensors.SensorLibrary
{
    [SensorUI(Category = "WMI", Tooltip = "Measure disk statistics via WMI.")]
    public class WmiDiskSensor : BaseSensor
    {
        [AutoUI(Group = "Drive")]
        public string DriveLetter { get; set; }

        [AutoUI(Group = "Drive")]
        public DiskMeasurement Measurement { get; set; }

        #region Implementation
        public override async Task<CollectResult> Collect()
        {
            var coll = WmiHelper.WmiQuery(Device, String.Format("SELECT * FROM Win32_LogicalDisk WHERE Name = '{0}:'", DriveLetter));

            // Count usage of each OS instance (should really be only one!)
            decimal total = 0;
            decimal free = 0;
            int count = 0;
            foreach (ManagementObject queryObj in coll) {
                object o = queryObj["FreeSpace"];
                count++;
                free += (decimal)Convert.ChangeType(o, typeof(decimal));
                o = queryObj["Size"];
                count++;
                total += (decimal)Convert.ChangeType(o, typeof(decimal));
            }

            // Average them out
            if (count > 0) {
                switch (Measurement) {
                    case DiskMeasurement.BytesFree:
                        return new CollectResult(free);
                    case DiskMeasurement.BytesUsed:
                        return new CollectResult(total - free);
                    case DiskMeasurement.MegabytesFree:
                        return new CollectResult(free / 1024 / 1024);
                    case DiskMeasurement.MegabytesUsed:
                        return new CollectResult((total - free) / 1024 / 1024);
                    case DiskMeasurement.PercentFree:
                        return new CollectResult((free / total) * 100);
                    case DiskMeasurement.PercentUsed:
                        return new CollectResult(1 - (free / total) * 100);
                }
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
