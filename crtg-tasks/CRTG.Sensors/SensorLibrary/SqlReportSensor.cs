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
using System.Data.SqlClient;
using System.Data;
using CRTG.Common;
using CRTG.Common.Attributes;
using CRTG.Common.Data;
using System.Threading.Tasks;

namespace CRTG.Sensors.SensorLibrary
{
    [SensorUI(Category = "SQL", Tooltip = "Run a complex query and email that report to a list of recipients.")]
    public class SqlReportSensor : BaseSensor
    {
        /// <summary>
        /// The SQL command to be executed to track this data
        /// </summary>
        [AutoUI(Group = "SQL", MultiLine = 10)]
        public string Sql { get; set; }

        /// <summary>
        /// The number of seconds to wait for a timeout
        /// </summary>
        [AutoUI(Group = "SQL", Help="Select '0' for unlimited timeout.")]
        public int TimeoutSeconds { get; set; }

        [AutoUI(Group = "Report")]
        public string ReportRecipients { get; set; }

        [AutoUI(Group = "Report")]
        public string ReportSubject { get; set; }

        [AutoUI(Group = "Report", MultiLine = 10)]
        public string ReportMessage { get; set; }

        [AutoUI(Group = "Report", Help="A comma-separated list of the days of the month when this report should be sent.")]
        public string DaysOfMonth { get; set; }

        [AutoUI(Group = "Report", Help = "In what format should this report be sent?")]
        public ReportFileFormat ReportFormat { get; set; }


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

            // Skip out if theres no work to do
            if (!should_run) return new CollectResult(0);

            // Connect to the database and retrieve this value
            DataTable dt = null;
            using (SqlConnection conn = new SqlConnection(this.Device.ConnectionString)) {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(Sql, conn)) {
                    cmd.CommandTimeout = TimeoutSeconds;
                    SqlDataReader dr = cmd.ExecuteReader();
                    dt = new DataTable();
                    dt.Load(dr);
                    dr.Close();
                }
                conn.Close();
            }

            // Here's our report
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
