using CRTG.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;
using System.IO;

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
            _connstring = String.Format("Data Source={0};Version=3;Pooling=True;Max Pool Size=100;Compress=true;", _sqlite_filename);
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

                // First, does the database exist?  If not, create it
                if (!File.Exists(_sqlite_filename)) {
                    SQLiteConnection.CreateFile(_sqlite_filename);
                }

                // Open it
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
            using (var conn = new SQLiteConnection(_connstring)) {
                conn.Open();

                // Insert using raw compiled code
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = @"INSERT INTO exceptions (sensor_id, exception_time, exception_text, stacktrace, cleared) values (@sensor_id, @exception_time, @exception_text, @stacktrace, @cleared)";

                    // Set up parameters
                    string[] parameter_names = new[] { "@sensor_id", "@exception_time", "@exception_text", "@stacktrace", "@cleared" };
                    DbParameter[] parameters = parameter_names.Select(pn =>
                    {
                        DbParameter parameter = cmd.CreateParameter();
                        parameter.ParameterName = pn;
                        cmd.Parameters.Add(parameter);
                        return parameter;
                    }).ToArray();

                    // Iterate through the objects in this list
                    parameters[0].Value = sensor.Identity;
                    parameters[1].Value = exception_to_save.ExceptionTime.Ticks;
                    parameters[2].Value = exception_to_save.Description;
                    parameters[3].Value = exception_to_save.StackTrace;
                    parameters[4].Value = exception_to_save.Cleared ? 1 : 0;
                    cmd.ExecuteNonQuery();
                }
            }
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
            // Create a new sensor data collection object
            SensorDataCollection result = new SensorDataCollection();
            result.StartDate = DateTime.MinValue;
            result.EndDate = DateTime.MaxValue;

            // Start the query process
            using (var conn = new SQLiteConnection(_connstring)) {
                conn.Open();

                // Insert using raw compiled code
                using (var cmd = conn.CreateCommand()) {

                    // Set up parameters
                    string[] parameter_names = new[] { "@sensor_id", "@start_date", "@end_date" };
                    DbParameter[] parameters = parameter_names.Select(pn =>
                    {
                        DbParameter parameter = cmd.CreateParameter();
                        parameter.ParameterName = pn;
                        cmd.Parameters.Add(parameter);
                        return parameter;
                    }).ToArray();
                    
                    // Next set up the actual query text
                    string measurement_query = @"SELECT measurement_time, value, collection_time_ms FROM measurements WHERE sensor_id = @sensor_id";
                    string exception_query = @"SELECT exception_time, exception_text, stacktrace, cleared FROM exceptions WHERE sensor_id = @sensor_id";
                    parameters[0].Value = sensor.Identity;
                    if (start_date != null && start_date.HasValue) {
                        measurement_query += " AND measurement_time >= @start_date";
                        exception_query += " AND exception_time >= @start_date";
                        parameters[1].Value = start_date.Value.Ticks;
                        result.StartDate = start_date.Value;
                    }
                    if (end_date != null && end_date.HasValue) {
                        measurement_query += " AND measurement_time <= @end_date";
                        exception_query += " AND exception_time <= @end_date";
                        parameters[2].Value = end_date.Value.Ticks;
                        result.EndDate = end_date.Value;
                    }

                    // Now query for measurements
                    cmd.CommandText = measurement_query;
                    using (var dr = cmd.ExecuteReader()) {
                        while (dr.Read()) {
                            SensorData sd = new SensorData();
                            sd.Time = new DateTime(dr.GetInt64(0));
                            sd.Value = dr.GetDecimal(1);
                            sd.CollectionTimeMs = dr.GetInt32(2);
                            result.Data.Add(sd);
                        }
                    }

                    // Now reset the query for the exceptions table
                    cmd.CommandText = exception_query;
                    using (var dr = cmd.ExecuteReader()) {
                        while (dr.Read()) {
                            SensorException se = new SensorException();
                            se.ExceptionTime = new DateTime(dr.GetInt64(0));
                            se.Description = dr.GetString(1);
                            se.StackTrace = dr.GetString(2);
                            se.Cleared = (dr.GetInt32(3) == 1);
                            result.Exceptions.Add(se);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Delete all data for this sensor
        /// </summary>
        /// <param name="sensor"></param>
        public void DeleteSensorData(ISensor sensor)
        {
            throw new Exception("Can't delete data yet");
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
