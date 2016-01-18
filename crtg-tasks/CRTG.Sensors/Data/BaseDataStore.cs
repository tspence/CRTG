using CRTG.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Sensors.Data
{
    public class BaseDataStore : IDataStore
    {
        /// <summary>
        /// Create data in bulk, overwriting any previous data if any existed
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="data_to_save"></param>
        public virtual void CreateData(ISensor sensor, SensorDataCollection data_to_save)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Append one item of data
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="data_to_save"></param>
        public virtual void AppendData(ISensor sensor, SensorData data_to_save)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Append one item of data
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="data_to_save"></param>
        public virtual void AppendData(ISensor sensor, SensorException exception_to_save)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Fetch some data for this sensor
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="start_date"></param>
        /// <param name="end_date"></param>
        /// <param name="fetch_exceptions"></param>
        /// <returns></returns>
        public virtual SensorDataCollection RetrieveData(ISensor sensor, DateTime? start_date = null, DateTime? end_date = null, bool fetch_exceptions = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete all data for this sensor
        /// </summary>
        /// <param name="sensor"></param>
        public virtual void DeleteSensorData(ISensor sensor)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove cached information, if any, from this data store
        /// </summary>
        public virtual void FlushCache()
        {
            throw new NotImplementedException();
        }
    }
}
