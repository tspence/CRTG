using CSVFile;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Notification
{
    public class CsvAttachmentHelper
    {
        /// <summary>
        /// Construct a CSV attachment for the email
        /// </summary>
        /// <param name="report_data"></param>
        /// <param name="attachment_filename"></param>
        /// <returns></returns>
        public static Attachment BuildAttachment(DataTable report_data, string attachment_filename)
        {
            string csvstring = report_data.WriteToString(true);
            var bytes = Encoding.UTF8.GetBytes(csvstring);
            Attachment a = new Attachment(new MemoryStream(bytes), new System.Net.Mime.ContentType("application/csv"));
            a.Name = attachment_filename;
            return a;
        }
    }
}
