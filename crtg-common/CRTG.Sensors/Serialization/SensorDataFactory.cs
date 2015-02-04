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
            return new CSVSensorDataFile(csv_fn);
        }
        #endregion
    }
}
