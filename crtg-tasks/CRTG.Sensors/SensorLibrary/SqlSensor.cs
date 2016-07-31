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
using System.Xml;
using System.Data.SqlClient;
using CRTG.Common;

namespace CRTG.Sensors.SensorLibrary
{
    [SensorUI(Category = "SQL", Tooltip = "Run a simple query and measure the results.")]
    public class SqlSensor : BaseSensor
    {
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

        #region Implementation
        public override decimal Collect()
        {
            Decimal d = 0;

            // Connect to the database and retrieve this value
            using (SqlConnection conn = new SqlConnection(this.Device.ConnectionString)) {
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
        #endregion
    }
}
