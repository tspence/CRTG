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
using System.IO;
using CSVFile;
using System.Data;
using CRTG.Common;
using CRTG.Sensors.Data;
using CRTG.Common.Interfaces;

namespace CRTG.Sensors.Data
{
    public class CSVSensorDataStore : IDataStore
    {
        protected string _csv_folder_path;

        public CSVSensorDataStore(string csv_folder_path)
        {
            _csv_folder_path = csv_folder_path;
            if (String.IsNullOrEmpty(_csv_folder_path)) {
                _csv_folder_path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),"SensorData");
            }
            if (!Directory.Exists(_csv_folder_path)) {
                Directory.CreateDirectory(_csv_folder_path);
            }
        }

        public SensorDataCollection RetrieveData(ISensor sensor, DateTime? start_date = null, DateTime? end_date = null, bool fetch_exceptions = false)
        {
            var data = new SensorDataCollection();
            data.StartDate = start_date ?? DateTime.MinValue;
            data.EndDate = end_date ?? DateTime.MaxValue;
            data.Data = new List<SensorData>();
            data.Exceptions = new List<SensorException>();

            // Check to see if this sensor filename exists
            string fn = GetFilename(sensor);
            if (File.Exists(fn)) {

                // Construct a new CSVReader and be careful about this
                using (var csv = new CSVReader(fn, ',', '\"', false)) {
                    foreach (string[] line in csv) {
                        if (line != null && line.Length >= 2) {
                            DateTime t = DateTime.MinValue;
                            Decimal val;

                            // Load the measurement time if present
                            int ms = 0;
                            if (line.Length >= 3)  int.TryParse(line[2], out ms); 

                            // Save values
                            if (DateTime.TryParse(line[0], out t) && Decimal.TryParse(line[1], out val)) {
                                
                                // Only load the specific timeframe
                                if (t < data.StartDate || t > data.EndDate) continue;

                                // Load the exception if asked
                                if (fetch_exceptions) {
                                    if (line.Length >= 3) {
                                        SensorException e = new SensorException()
                                        {
                                            Description = line[2],
                                            ExceptionTime = t
                                        };
                                        data.Exceptions.Add(e);
                                    }
                                }

                                // Load the sensor data
                                SensorData sd = new SensorData()
                                {
                                    Time = t,
                                    Value = val,
                                    CollectionTimeMs = ms
                                };
                                data.Data.Add(sd);
                            }
                        }
                    }
                }
            }

            // Mark for lazy loading
            return data;
        }

        /// <summary>
        /// Retrieve the filename for this particular sensor
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        private string GetFilename(ISensor sensor)
        {
            string fn = Path.Combine(_csv_folder_path, sensor.Identity.ToString() + ".csv");
            return fn;
        }

        /// <summary>
        /// Create data in bulk, overwriting data that was already there, if any
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="data_to_save"></param>
        public void CreateData(ISensor sensor, SensorDataCollection data_to_save)
        {
            // Find the sensor file
            string fn = GetFilename(sensor);
            if (File.Exists(fn)) {
                File.Delete(fn);
            }

            // Convert to an array to avoid list conflicts
            var array = data_to_save.Data.ToArray();
            array.WriteToStream(fn, true);
        }

        /// <summary>
        /// Append single data
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="data_to_save"></param>
        public void AppendData(ISensor sensor, SensorData data_to_save)
        {
            string fn = GetFilename(sensor);
            string csv = CSV.SaveArray<SensorData>(new SensorData[] { data_to_save }, false, false);
            File.AppendAllText(fn, csv);
        }

        /// <summary>
        /// Append one item of data
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="data_to_save"></param>
        public void AppendData(ISensor sensor, SensorException exception_to_save)
        {
        }

        /// <summary>
        /// Delete all data for this sensor
        /// </summary>
        /// <param name="sensor"></param>
        public void DeleteSensorData(ISensor sensor)
        {
        }

        /// <summary>
        /// Remove cached information, if any, from this data store
        /// </summary>
        public void FlushCache()
        {
        }
    }
}
