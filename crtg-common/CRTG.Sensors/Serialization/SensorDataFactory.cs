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
            if (!Directory.Exists("sensors")) Directory.CreateDirectory("Sensors");
            string csv_fn = String.Format("sensors\\sensor.{0}.csv", Identity);
            return new CSVSensorDataFile(csv_fn);
        }
        #endregion
    }
}
