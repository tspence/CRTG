/*
 * 2012 - 2015 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/CRTG
 * 
 * This program uses icons from http://www.famfamfam.com/lab/icons/silk/
 */
using CSVFile;
using CRTG.Charts;
using CRTG.Sensors;
using CRTG.Sensors.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace CRTG.Notification
{
    public class NotificationHelper : BaseNotificationSystem
    {
        #region Implementation Details
        /// <summary>
        /// Public notification API
        /// </summary>
        /// <param name="method"></param>
        /// <param name="notify_type"></param>
        /// <param name="data"></param>
        /// <param name="sensor"></param>
        /// <param name="message"></param>
        public override void Notify(BaseSensor sensor, NotificationState notify_type, DateTime timestamp, decimal data, string message)
        {
            // Parse this out to the correct notification method
            switch (sensor.Method) {
                case NotificationMethod.Email:
                    NotifyEmail(sensor, notify_type, timestamp, data, message);
                    break;
            }
        }

        /// <summary>
        /// Deliver a preprogrammed report
        /// </summary>
        /// <param name="recipients"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="report_data"></param>
        /// <param name="attachment_filename"></param>
        public override void SendReport(string[] recipients, string subject, string message, DataTable report_data, ReportFileFormat format, string attachment_filename)
        {
            // Do we have a real list of recipients?  Are all the recipients empty?
            if (recipients == null) return;
            var any_real_recipients = (from r in recipients where !String.IsNullOrWhiteSpace(r) select r).Any();
            if (!any_real_recipients) return;

            // Do everything carefully
            MailMessage msg = null;
            try {

                // Put together the mail message
                msg = new MailMessage();
                if (String.IsNullOrEmpty(SensorProject.Current.MessageFrom)) {
                    msg.From = new MailAddress("crtg-noreply@localhost.com");
                } else {
                    msg.From = new MailAddress(SensorProject.Current.MessageFrom);
                }
                msg.Subject = subject;


                // List of recipients (to)
                foreach (string s in recipients) {
                    if (!String.IsNullOrEmpty(s)) {
                        msg.To.Add(s);
                    }
                }

                // Produce safe filename
                char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
                var validFilename = new string(attachment_filename.Where(ch => !invalidFileNameChars.Contains(ch)).ToArray());
                validFilename = validFilename.Replace(' ', '_');

                // Insert all the attachments
                Attachment a = null;
                string html_insert = null;
                switch (format) {
                    case ReportFileFormat.CSV:
                        a = CsvAttachmentHelper.BuildAttachment(report_data, attachment_filename + ".csv");
                        break;
                    case ReportFileFormat.ExcelWithTables:
                        a = ExcelAttachmentHelper.BuildAttachment(report_data, attachment_filename + ".xlsx", attachment_filename);
                        break;
                    case ReportFileFormat.InlineHtml:
                        a = null;
                        html_insert = HtmlAttachmentHelper.BuildAttachment(report_data);
                        break;
                    case ReportFileFormat.OpenXML:
                        a = OpenDocumentAttachmentHelper.BuildAttachment(report_data, attachment_filename + ".xlsx");
                        break;
                }
                if (a != null) {
                    msg.Attachments.Add(a);
                }

                // Set the body of the message
                msg.Body = "<html><head></head><body>" + message + "<br/><br/>" + html_insert + "</body></html>";
                msg.IsBodyHtml = true;

                // Create the SMTP client and deliver the message
                SmtpClient SmtpClient = new SmtpClient(SensorProject.Current.SmtpHost);

                // Provides credentials for password-based authentication schemes such as basic, digest, NTLM, and Kerberos authentication.
                if (SensorProject.Current.SmtpUsername != null) {
                    SmtpClient.UseDefaultCredentials = false;
                    SmtpClient.Credentials = new System.Net.NetworkCredential(SensorProject.Current.SmtpUsername, SensorProject.Current.SmtpPassword);
                }

                // Let's send it
                SmtpClient.Send(msg);

            } catch (Exception ex) {
                SensorProject.LogException("NotificationHelper.SendReport", ex);
            } finally {
                if (msg != null) msg.Dispose();
            }
        }
        #endregion

        #region Helper Functions
        /// <summary>
        /// Fix up a message that contains embedded tokens (in the form "@TOKEN@") with the correct values
        /// </summary>
        /// <param name="message"></param>
        /// <param name="device"></param>
        /// <param name="sensor"></param>
        /// <returns></returns>
        protected string FixupMessage(string text, BaseSensor sensor, NotificationState notify_type, DateTime timestamp, decimal data, string message)
        {
            return text
                .Replace("@DEVICE@", sensor.Device.DeviceName)
                .Replace("@DEVICEINFO@", sensor.Device.DeviceInformation)
                .Replace("@SENSOR@", sensor.Name)
                .Replace("@MESSAGE@", message)
                .Replace("@CONDITION@", notify_type.ToString())
                .Replace("@VALUE@", data.ToString())
                .Replace("@TIMESTAMP@", timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// Trigger the email message
        /// </summary>
        /// <param name="device"></param>
        /// <param name="sensor"></param>
        /// <param name="chart"></param>
        /// <param name="message"></param>
        protected void NotifyEmail(BaseSensor sensor, NotificationState notify_type, DateTime timestamp, decimal data, string message)
        {
            string chart_attachment_file = null;

            // Do everything carefully
            try {

                // Use default informatoin if none provided
                string subj = SensorProject.Current.SubjectLineTemplate;
                if (String.IsNullOrEmpty(subj) || subj.Trim().Length == 0) {
                    subj = "CRTG: @DEVICE@ - @SENSOR@ - @CONDITION@";
                }
                string body = SensorProject.Current.MessageBodyTemplate;
                if (String.IsNullOrEmpty(body) || body.Trim().Length == 0) {
                    body = @"
<html>
    <body>
        <h2>CRTG: @CONDITION@ - @SENSOR@</h2>
        <ul>
            <li>Condition: @CONDITION@</li>
            <li>Device: @DEVICE@</li>
            <li>Info: @DEVICEINFO@</li>
            <li>Sensor: @SENSOR@</li>
            <li>Value: @VALUE@</li>
            <li>Message: @MESSAGE@</li>
            <li>Timestamp: @TIMESTAMP@</li>
        </ul>

        @CHART@
    </body>
</html>";
                }
                string fromaddr = SensorProject.Current.MessageFrom;
                if (string.IsNullOrEmpty(fromaddr)) {
                    fromaddr = "crtg-noreply@localhost.com";
                }

                // Does this message require a chart attachment?
                if (body.Contains("@CHART@")) {
                    chart_attachment_file = ChartHelper.GetChartImage(sensor, ViewTimeframe.Day, 500, 300);
                    body = body.Replace("@CHART@", String.Format("<img src=\"cid:{0}\"/>", Path.GetFileName(chart_attachment_file)));
                }

                // Put together the mail message
                SendEmail(sensor.Recipients.Split(','),
                    FixupMessage(subj, sensor, notify_type, timestamp, data, message),
                    FixupMessage(body, sensor, notify_type, timestamp, data, message),
                    new string[] { chart_attachment_file });

            // We really should log weird exceptions somewhere
            } catch (Exception ex) {
                SensorProject.LogException("NotificationHelper.NotifyEmail", ex);
            }

            // Now clean up all the attachment files we created
            if (chart_attachment_file != null) {
                File.Delete(chart_attachment_file);
            }
        }

        /// <summary>
        /// Make a guess based on the file's extension what mime type to use
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static string GetMimeType(string filename)
        {
            // Default to unknown binary file format
            string mimeType = "application/octet-stream";

            // Look in the registry - does this extension exist anywhere?
            RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(Path.GetExtension(filename).ToLower());
            if (regKey != null) {
                object contentType = regKey.GetValue("Content Type");
                if (contentType != null) {
                    mimeType = contentType.ToString();
                }
            }

            // This is our best guess
            return mimeType;
        }

        /// <summary>
        /// Method for sending emails using system defined notification settings
        /// </summary>
        /// <param name="ReportRecipients"></param>
        /// <param name="ReportSubject"></param>
        /// <param name="ReportMessage"></param>
        /// <param name="p"></param>
        public void SendEmail(string[] recipients, string subject, string message, string[] attachment_filenames)
        {
            // Do we have a real list of recipients?  Are all the recipients empty?
            if (recipients == null) return;
            var any_real_recipients = (from r in recipients where !String.IsNullOrWhiteSpace(r) select r).Any();
            if (!any_real_recipients) return;

            // Do everything carefully
            MailMessage msg = null;
            try {

                // Put together the mail message
                msg = new MailMessage();
                if (String.IsNullOrEmpty(SensorProject.Current.MessageFrom)) {
                    msg.From = new MailAddress("crtg-noreply@localhost.com");
                } else {
                    msg.From = new MailAddress(SensorProject.Current.MessageFrom);
                }
                msg.Subject = subject;
                msg.Body = message;
                if (msg.Body.IndexOf("<html>", StringComparison.CurrentCultureIgnoreCase) >= 0) {
                    msg.IsBodyHtml = true;
                }

                // List of recipients (to)
                foreach (string s in recipients) {
                    if (!String.IsNullOrEmpty(s)) {
                        msg.To.Add(s);
                    }
                }

                // Insert all the attachments
                foreach (string fn in attachment_filenames) {
                    if (File.Exists(fn)) {
                        string mime_type = GetMimeType(fn);
                        Attachment a = new Attachment(fn, mime_type);
                        msg.Attachments.Add(a);
                    }
                }

                // Create the SMTP client and deliver the message
                SmtpClient SmtpClient = new SmtpClient(SensorProject.Current.SmtpHost);

                // Provides credentials for password-based authentication schemes such as basic, digest, NTLM, and Kerberos authentication.
                if (SensorProject.Current.SmtpUsername != null) {
                    SmtpClient.UseDefaultCredentials = false;
                    SmtpClient.Credentials = new System.Net.NetworkCredential(SensorProject.Current.SmtpUsername, SensorProject.Current.SmtpPassword);
                }

                // Let's send it
                SmtpClient.Send(msg);

            } catch (Exception ex) {
                SensorProject.LogException("NotificationHelper.SendEmail", ex);
            } finally {
                if (msg != null) msg.Dispose();
            }
        }



        #endregion
    }
}
