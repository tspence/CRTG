using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CRTG.Serialization
{
    public class SensorDataFactory
    {
        #region Public Interface
        /// <summary>
        /// Retrieve data from the file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static BaseSensorDataFile GetFile(int Identity)
        {
            string csv_fn = String.Format("sensor.{0}.csv", Identity);
            //string binary_fn = String.Format("sensor.{0}.bin", Identity);
            //BinarySensorDataFile bin = new BinarySensorDataFile(binary_fn);

            // If the binary file exists, just use that
            //if (File.Exists(binary_fn)) {
            //    bin.Load();

            // No binary file - check for presence of an oldschool CSV file, and upgrade it if present
            //} else if (File.Exists(csv_fn)) {
                CSVSensorDataFile csv = new CSVSensorDataFile(csv_fn);
                csv.Load();
                return csv;
            //    bin.Data = csv.Data;
            //    bin.Save();
            //}

            // We only give binary file formats now
            //return bin;
        }
        #endregion
    }
}
