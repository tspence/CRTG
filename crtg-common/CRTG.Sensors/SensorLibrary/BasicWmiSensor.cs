using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace CRTG.Sensors.SensorLibrary
{
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
