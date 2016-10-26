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
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CRTG.Sensors.SensorLibrary
{
    [SensorUI(Category = "MySQL", Tooltip = "Run a report query and deliver the results to a recipient list via email or upload.")]
    public class MySqlReportSensor : BaseSensor
    {
        /// <summary>
        /// The SQL command to be executed to track this data
        /// </summary>
        [AutoUI(Group = "SQL", MultiLine = 10)]
        public string Sql { get; set; }

        /// <summary>
        /// The number of seconds to wait for a timeout
        /// </summary>
        [AutoUI(Group = "SQL", Help = "Select '0' for unlimited timeout.")]
        public int TimeoutSeconds { get; set; }

        /// <summary>
        /// Which days of the month do we run this report
        /// </summary>
        [AutoUI(Group = "Report", Help = "A comma-separated list of the days of the month when this report should be sent.  Leave blank for all days.")]
        public string DaysOfMonth { get; set; }


        #region Implementation
        public override async Task<CollectResult> Collect()
        {
            // Determine if this report should run as per this day of the month
            string day = DateTime.UtcNow.Day.ToString();
            bool should_run = false;
            if (String.IsNullOrEmpty(DaysOfMonth)) {
                should_run = true;
            } else {
                List<string> days = DaysOfMonth.Split(',').ToList();
                should_run = days.Contains(day);
            }

            // Connect to the database and retrieve this value
            DataTable dt = null;
            using (MySqlConnection conn = new MySqlConnection(this.Device.ConnectionString)) {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(Sql, conn)) {
                    cmd.CommandTimeout = TimeoutSeconds;
                    MySqlDataReader dr = cmd.ExecuteReader();
                    dt = new DataTable();
                    dt.Load(dr);
                    dr.Close();
                }
                conn.Close();
            }

            // That's our result
            return new CollectResult(dt.Rows.Count, dt);
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
