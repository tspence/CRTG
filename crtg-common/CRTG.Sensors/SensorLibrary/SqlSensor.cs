/*
 * 2006 - 2012 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://code.google.com/p/csharp-striker/
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data.SqlClient;

namespace CRTG.Sensors.SensorLibrary
{
    public class SqlSensor : BaseSensor
    {
        /// <summary>
        /// The connection string to be used for this connection
        /// </summary>
        [AutoUI(Group = "SQL")]
        public string ConnectionString;

        /// <summary>
        /// The SQL command to be executed to track this data
        /// </summary>
        [AutoUI(Group = "SQL",MultiLine=10)]
        public string Sql;

        /// <summary>
        /// What measurement to take
        /// </summary>
        [AutoUI(Group = "SQL")]
        public SqlCollectionType Measurement = SqlCollectionType.ScalarValue;

        public override decimal Collect()
        {
            Decimal d = 0;

            // Connect to the database and retrieve this value
            using (SqlConnection conn = new SqlConnection(ConnectionString)) {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(Sql, conn)) {

                    // Use the appropriate measurement - this is for scalar
                    if (Measurement == SqlCollectionType.ScalarValue) {
                        object o = cmd.ExecuteScalar();
                        d = (decimal)Convert.ChangeType(o, typeof(decimal));

                    // This is for a query
                    } else if (Measurement == SqlCollectionType.RecordCount) {
                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read()) {
                            d = d + 1;
                        }
                        dr.Close();
                    }
                }
                conn.Close();
            }

            // That's our value
            return d;
        }
    }
}
