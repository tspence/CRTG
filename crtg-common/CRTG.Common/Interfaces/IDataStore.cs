using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Common
{
    public interface IDataStore
    {
        /// <summary>
        /// Create data in bulk, overwriting any previous data if any existed
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="data_to_save"></param>
        public void CreateData(ISensor sensor, SensorDataCollection data_to_save);

        /// <summary>
        /// Append one item of data
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="data_to_save"></param>
        public void AppendData(ISensor sensor, SensorData data_to_save);

        /// <summary>
        /// Append one item of data
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="data_to_save"></param>
        public void AppendData(ISensor sensor, SensorException exception_to_save);

        /// <summary>
        /// Fetch some data for this sensor
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="start_date"></param>
        /// <param name="end_date"></param>
        /// <param name="fetch_exceptions"></param>
        /// <returns></returns>
        public SensorDataCollection RetrieveData(ISensor sensor, DateTime? start_date = null, DateTime? end_date = null, bool fetch_exceptions = false);

        /// <summary>
        /// Delete all data for this sensor
        /// </summary>
        /// <param name="sensor"></param>
        public void DeleteSensorData(ISensor sensor);

        /// <summary>
        /// Remove cached information, if any, from this data store
        /// </summary>
        public void FlushCache();
    }
}
