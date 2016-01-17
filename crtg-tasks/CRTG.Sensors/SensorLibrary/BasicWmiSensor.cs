/*
 * 2012 - 2015 Ted Spence, http://tedspence.com
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

namespace CRTG.Sensors.SensorLibrary
{
    [SensorUI(Category="WMI", Tooltip="Create a basic WMI sensor that measures one of a handful of simple facts.")]
    public class BasicWmiSensor : BaseSensor
    {
        [AutoUI(Label="Measurement")]
        public BasicWmiQuery WmiQuery = BasicWmiQuery.CPU;

        public override decimal Collect()
        {
            ObjectQuery wql = null;
            ManagementObjectSearcher searcher = null;
            ManagementObjectCollection result = null;
            decimal total = 0;
            int count = 0;
            switch (WmiQuery) {

                // Determine level of free memory
                case BasicWmiQuery.Memory:
                    wql = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                    searcher = new ManagementObjectSearcher(wql);
                    result = searcher.Get();
                    foreach (ManagementObject mo in result) {
                        decimal total_mem = (decimal)mo["TotalVisibleMemorySize"];
                        decimal free_mem = (decimal)mo["FreePhysicalMemory"];
                        return free_mem / total_mem;
                    }
                    break;

                case BasicWmiQuery.CPU:
                    wql = new ObjectQuery("SELECT * FROM Win32_PerfRawData_PerfOS_Processor");
                    searcher = new ManagementObjectSearcher(wql);
                    result = searcher.Get();
                    foreach (ManagementObject queryObj in searcher.Get()) {
                        object o = queryObj["PercentIdleTime"];
                        count++;
                        total += (decimal)Convert.ChangeType(o, typeof(decimal));
                    }
                    if (count > 0) return (100 - (total / count));
                    throw new Exception("No CPUs found!");
            } 

            // Failed!
            throw new Exception("No way to collect");
        }
    }
}
