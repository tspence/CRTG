using CRTG.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Sensors.Data
{
    public class SqliteSensorException
    {
        public Int64 sensor_id { get; set; }
        public Int64 exception_time { get; set; }
        public String exception_text { get; set; }
        public String stacktrace { get; set; }
        public Int64 cleared { get; set; }

        /// <summary>
        /// Convert an object from the database for memory usage
        /// </summary>
        /// <returns></returns>
        public SensorException FromDatabaseFormat()
        {
            return new SensorException()
            {
                Description = exception_text,
                Cleared = (cleared == 1),
                ExceptionTime = new DateTime(exception_time),
                Item = new Exception(stacktrace)
            };
        }

        /// <summary>
        /// Convert an object to the database format
        /// </summary>
        /// <param name="sensor_id"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static SqliteSensorException ToDatabaseFormat(int sensor_id, SensorException e)
        {
            return new SqliteSensorException()
            {
                sensor_id = sensor_id,
                exception_time = e.ExceptionTime.Ticks,
                exception_text = e.Description,
                cleared = e.Cleared ? 1 : 0,
                stacktrace = e.Item.StackTrace,
            };
        }
    }
}
