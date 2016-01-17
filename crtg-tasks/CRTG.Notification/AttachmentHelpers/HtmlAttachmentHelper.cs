using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Notification
{
    public class HtmlAttachmentHelper
    {
        public static string BuildAttachment(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table>");

            // Set up header row
            sb.Append("<tr>");
            for (int col = 0; col < dt.Columns.Count; col++) {
                sb.Append("<th>");
                sb.Append(dt.Columns[col].ColumnName);
                sb.Append("</th>");
            }
            sb.Append("</tr>");

            // Set up data rows
            for (int row = 0; row < dt.Rows.Count; row++) {
                sb.Append("<tr>");
                var dr = dt.Rows[row];
                for (int col = 0; col < dt.Columns.Count; col++) {
                    sb.Append("<td>");
                    sb.Append(dr[col].ToString());
                    sb.Append("</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            // Here's your table
            return sb.ToString();
        }
    }
}
