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

namespace CRTG.UI.Helpers
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
        public override void SendReport(string[] recipients, string subject, string message, DataTable report_data, string attachment_filename)
        {
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

                // Produce safe filename
                char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
                var validFilename = new string(attachment_filename.Where(ch => !invalidFileNameChars.Contains(ch)).ToArray());
                validFilename = validFilename.Replace(' ', '_');

                // Insert all the attachments
                Attachment a = null;
                if (SensorProject.Current.ReportFileFormatPreference == ReportFileFormat.OpenXML) {
                    a = ConstructAttachmentFromData_OpenXML(report_data, attachment_filename + ".xlsx");
                } else {
                    a = ConstructAttachmentFromData_CSV(report_data, attachment_filename + ".csv");
                }
                msg.Attachments.Add(a);

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
                SensorProject.Log.ErrorFormat("Unable to send email:\r\n{0}", ex.ToString());
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
                SensorProject.Log.ErrorFormat("Unable to send email from sensor {0} condition {1}\r\n{2}", sensor.Name, notify_type.ToString(), ex.ToString());
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
                SensorProject.Log.ErrorFormat("Unable to send email:\r\n{0}", ex.ToString());
            } finally {
                if (msg != null) msg.Dispose();
            }
        }

        private Attachment ConstructAttachmentFromData_CSV(DataTable report_data, string attachment_filename)
        {
            string csvstring = report_data.WriteToString(true);
            var bytes = Encoding.UTF8.GetBytes(csvstring);
            Attachment a = new Attachment(new MemoryStream(bytes), new System.Net.Mime.ContentType("application/csv"));
            a.Name = attachment_filename;
            return a;
        }

        /// <summary>
        /// Shortcut to construct an attachment object using Excel open document file formats
        /// </summary>
        /// <param name="report_data"></param>
        /// <param name="attachment_filename"></param>
        /// <returns></returns>
        private Attachment ConstructAttachmentFromData_OpenXML(DataTable report_data, string attachment_filename)
        {
            string fn = Path.GetTempFileName() + ".xlsx";
            using (SpreadsheetDocument workbook = SpreadsheetDocument.Create(fn, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook, true)) {
                // Construct the workbook parts
                var workbookPart = workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
                var sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();
                workbook.WorkbookPart.Workbook.Sheets = sheets;

                // Set up this sheet
                var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);
                string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);
                uint sheetId = 1;
                if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0) {
                    sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                }

                // Make this sheet a table
                string name = report_data.TableName;
                if (String.IsNullOrWhiteSpace(name)) {
                    name = Path.GetFileNameWithoutExtension(attachment_filename);
                }
                DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = name };
                sheets.Append(sheet);

                // Set up header row
                DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                List<String> columns = new List<string>();
                foreach (System.Data.DataColumn column in report_data.Columns) {
                    columns.Add(column.ColumnName);

                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
                    headerRow.AppendChild(cell);
                }
                sheetData.AppendChild(headerRow);

                // Set up data rows
                foreach (System.Data.DataRow dsrow in report_data.Rows) {
                    DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    foreach (String col in columns) {
                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString());
                        newRow.AppendChild(cell);
                    }
                    sheetData.AppendChild(newRow);
                }

                // Add a "Table Part" for convenient viewing
                if (report_data.Columns.Count < 26) {

                    // Determine ranges
                    char toprightcolumn = Convert.ToChar(Convert.ToByte('A') + report_data.Columns.Count - 1);
                    string tablereference = "A1:" + toprightcolumn + (report_data.Rows.Count + 1).ToString();

                    // Construct table definition part
                    TableDefinitionPart tdp = sheetPart.AddNewPart<TableDefinitionPart>();

                    // Construct table parts
                    TableParts tps1 = new TableParts() { Count = (UInt32Value)1U };
                    TablePart tp = new TablePart() { Id = sheetPart.GetIdOfPart(tdp) };
                    tps1.Append(tp);

                    // Construct a table definition
                    Table table = new Table() { Id = (UInt32Value)1U, Name = "Table1", DisplayName = "Table1", Reference = tablereference, TotalsRowShown = false };

                    // Add autofilter
                    AutoFilter filter = new AutoFilter() { Reference = tablereference };

                    // Construct all columns
                    TableColumns col_list = new TableColumns() { Count = new UInt32Value((uint)report_data.Columns.Count) };
                    uint colnum = 1;
                    foreach (System.Data.DataColumn column in report_data.Columns) {
                        TableColumn tc = new TableColumn() { Id = new UInt32Value(colnum++), Name = column.ColumnName };
                        col_list.Append(tc);
                    }

                    // Set styles
                    TableStyleInfo style = new TableStyleInfo() { Name = "TableStyleMedium3", ShowFirstColumn = false, ShowLastColumn = false, ShowRowStripes = true, ShowColumnStripes = false };

                    // Add to table on sheet
                    table.Append(filter);
                    table.Append(col_list);
                    table.Append(style);
                    tdp.Table = table;

                    // Add "tps1" to the sheet
                    sheetPart.Worksheet.Append(tps1);
                }

                // Close the workbook and save it
                workbook.Close();
            }

            // Here's your attachment
            byte[] bytes = File.ReadAllBytes(fn);
            File.Delete(fn);
            Attachment a = new Attachment(new MemoryStream(bytes), new System.Net.Mime.ContentType("application/vnd.ms-excel"));
            a.Name = attachment_filename;
            return a;
        }
        #endregion
    }
}
