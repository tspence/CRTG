using CRTG.Common;
using CRTG.Common.Attributes;
using CRTG.Common.Data;
using MySql.Data.MySqlClient;
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
using System.Threading.Tasks;

namespace CRTG.Sensors.SensorLibrary
{
    [SensorUI(Category = "MySQL", Tooltip = "Run a simple query and measure the results.")]
    public class MySqlSensor : BaseSensor
    {
        /// <summary>
        /// The SQL command to be executed to track this data
        /// </summary>
        [AutoUI(Group = "SQL", MultiLine = 10)]
        public string Sql { get; set; }

        /// <summary>
        /// What measurement to take
        /// </summary>
        [AutoUI(Group = "SQL")]
        public SqlCollectionType Measurement { get; set; }


        #region Implementation
        public override CollectResult Collect()
        {
            Decimal d = 0;

            // Connect to the database and retrieve this value
            using (var conn = new MySqlConnection(this.Device.ConnectionString)) {
                conn.Open();
                using (var cmd = new MySqlCommand(Sql, conn)) {

                    // Use the appropriate measurement - this is for scalar
                    if (Measurement == SqlCollectionType.ScalarValue) {
                        object o = cmd.ExecuteScalar();
                        d = (decimal)Convert.ChangeType(o, typeof(decimal));

                        // This is for a query
                    } else if (Measurement == SqlCollectionType.RecordCount) {
                        MySqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read()) {
                            d = d + 1;
                        }
                        dr.Close();
                    }
                }
                conn.Close();
            }

            // That's our value
            return new CollectResult(d);
        }
        #endregion

        #region Icon
        public override string GetNormalIconPath()
        {
            return "Resources/database.png";
        }
        #endregion
    }
}
