using CRTG.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;

namespace CRTG.Sensors.Data
{
    public class SqliteDataStore : IDataStore
    {
        private string _sqlite_filename;
        private string _connstring;


        public SqliteDataStore(string filename)
        {
            _sqlite_filename = filename;

            // Create the database if it does not exist or cannot be opened
            string connstring = String.Format("Data Source={0};Version=3;Pooling=True;Max Pool Size=100;Compress=true;", _sqlite_filename);
            UpgradeDatabase();
        }

        #region Database helper code
        /// <summary>
        /// Upgrade a database to the latest available version.
        /// All changescripts must be repeatable.
        /// </summary>
        /// <param name="conn"></param>
        private void UpgradeDatabase()
        {
            SQLiteConnection conn = null;
            try {
                using (conn = new SQLiteConnection(_connstring)) {
                    conn.Open();

                    // Create initial tables and indexes
                    conn.Execute(Resource1.schema_measurements_table);
                    conn.Execute(Resource1.schema_exceptions_table);
                    conn.Execute(Resource1.schema_measurements_index);
                    conn.Execute(Resource1.schema_exceptions_index);
                }
            } catch (Exception ex) {
                SensorProject.LogException("Upgrading database to latest version", ex);
            }
        }

        /// <summary>
        /// Insert objects into the database that have already been converted to the proper format
        /// </summary>
        /// <param name="list"></param>
        private void InsertItems(int sensor_id, List<SensorData> list)
        {
            using (var conn = new SQLiteConnection(_connstring)) {
                conn.Open();

                // Insert using raw compiled code
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = @"INSERT INTO measurements (sensor_id, measurement_time, value, collection_time_ms) values (@sensor_id, @measurement_time, @value, @collection_time_ms)";

                    // Set up parameters
                    string[] parameter_names = new[] { "@sensor_id", "@measurement_time", "@value", "@collection_time_ms" };
                    DbParameter[] parameters = parameter_names.Select(pn =>
                    {
                        DbParameter parameter = cmd.CreateParameter();
                        parameter.ParameterName = pn;
                        cmd.Parameters.Add(parameter);
                        return parameter;
                    }).ToArray();

                    // Iterate through the objects in this list
                    foreach (var d in list) {
                        parameters[0].Value = sensor_id;
                        parameters[1].Value = d.Time.Ticks;
                        parameters[2].Value = d.Value;
                        parameters[3].Value = d.CollectionTimeMs;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Create data in bulk, overwriting any previous data if any existed
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="data_to_save"></param>
        public void CreateData(ISensor sensor, SensorDataCollection data_to_save)
        {
            // Now insert using SQLite/Dapper
            InsertItems(sensor.Identity, data_to_save.Data);
        }

        /// <summary>
        /// Append one item of data
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="data_to_save"></param>
        public void AppendData(ISensor sensor, SensorData data_to_save)
        {
            // Now insert using SQLite/Dapper
            var list = new List<SensorData>();
            list.Add(data_to_save);
            InsertItems(sensor.Identity, list);
        }

        /// <summary>
        /// Append one item of data
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="data_to_save"></param>
        public void AppendData(ISensor sensor, SensorException exception_to_save)
        {
            throw new Exception("Can't save exceptions yet");
        }

        /// <summary>
        /// Fetch some data for this sensor
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="start_date"></param>
        /// <param name="end_date"></param>
        /// <param name="fetch_exceptions"></param>
        /// <returns></returns>
        public SensorDataCollection RetrieveData(ISensor sensor, DateTime? start_date = null, DateTime? end_date = null, bool fetch_exceptions = false)
        {
            return null;
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
            // Nothing to do - we don't cache since the sqlite database is good enough on its own
        }
    }
}
