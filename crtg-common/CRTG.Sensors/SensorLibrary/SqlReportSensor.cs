﻿/*
 * 2012 - 2015 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/CRTG
 * 
 * This program uses icons from http://www.famfamfam.com/lab/icons/silk/
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Serialization;
using CSVFile;
using System.IO;
using System.Net;

namespace CRTG.Sensors.SensorLibrary
{
    public class SqlReportSensor : BaseSensor
    {
        /// <summary>
        /// The SQL command to be executed to track this data
        /// </summary>
        [AutoUI(Group = "SQL", MultiLine = 10)]
        public string Sql;

        /// <summary>
        /// The number of seconds to wait for a timeout
        /// </summary>
        [AutoUI(Group = "SQL", Help="Select '0' for unlimited timeout.")]
        public int TimeoutSeconds;

        [AutoUI(Group = "Report")]
        public string ReportRecipients;

        [AutoUI(Group = "Report")]
        public string ReportSubject;

        [AutoUI(Group = "Report", MultiLine = 10)]
        public string ReportMessage;

        [AutoUI(Group = "Report", Help="A comma-separated list of the days of the month when this report should be sent.")]
        public string DaysOfMonth;

        #region Implementation
        public override decimal Collect()
        {
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

            // Generate a report email, if this day is within the specified day of the month
            if (dt != null) {

                // Determine if this report should run as per this day of the month
                string day = DateTime.UtcNow.Day.ToString();
                bool should_run = false;
                if (String.IsNullOrEmpty(DaysOfMonth)) {
                    should_run = true;
                } else {
                    List<string> days = DaysOfMonth.Split(',').ToList();
                    should_run = days.Contains(day);
                }

                // Do we run it?
                if (should_run) {
                    if (SensorProject.Current.Notifications != null) {

                        // Send report
                        SensorProject.Current.Notifications.SendReport(ReportRecipients.Split(','), ReportSubject, ReportMessage, dt, this.Name);
                    }
                }

                // Upload this report (if desired)
                if (UploadUrl != null) {
                    string csv = dt.WriteToString(false);
                    byte[] raw = Encoding.UTF8.GetBytes(csv);
                    ReportUploader.UploadReport(raw, UploadUrl, UploadUsername, UploadPassword);
                    LastUploadTime = DateTime.UtcNow;
                }

                // That's our value
                return dt.Rows.Count;
            }

            // Datatable failed to load - we got nothing.  Just report zero.
            return 0;
        }

        protected override void UploadCollection()
        {
            // Nothing to do here - we do not upload measurement stats on an SQL report sensor
        }
        #endregion
    }
}
