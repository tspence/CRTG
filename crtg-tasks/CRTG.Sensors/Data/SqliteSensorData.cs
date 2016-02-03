using CRTG.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Sensors.Data
{
    public class SqliteSensorData
    {
        public Int64 sensor_id { get; set; }
        public Int64 measurement_time { get; set; }
        public decimal value { get; set; }
        public Int64 collection_time_ms { get; set; }

        /// <summary>
        /// Convert an object from the database for memory usage
        /// </summary>
        /// <returns></returns>
        public SensorData FromDatabaseFormat()
        {
            return new SensorData()
            {
                Time = new DateTime(measurement_time),
                Value = value,
                CollectionTimeMs = (int)collection_time_ms
            };
        }

        /// <summary>
        /// Convert an object to the database format
        /// </summary>
        /// <param name="sensor_id"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static SqliteSensorData ToDatabaseFormat(int sensor_id, SensorData d)
        {
            return new SqliteSensorData()
            {
                sensor_id = sensor_id,
                measurement_time = d.Time.Ticks,
                value = d.Value,
                collection_time_ms = d.CollectionTimeMs
            };
        }
    }
}
