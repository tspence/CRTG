/*
 * 2012 - 2015 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/CRTG
 * 
 * This program uses icons from http://www.famfamfam.com/lab/icons/silk/
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CSVFile;

namespace CRTG.Sensors
{
    public class ReportUploader
    {
        /// <summary>
        /// Upload this report to a URL data source on the Internet
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="url"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static bool UploadReport(byte[] contents, string url, string username, string password)
        {
            // Is this object intended to upload to a web resource?
            try {
                WebRequest wr = WebRequest.Create(url);
                wr.Method = "POST";

                // Add credentials
                wr.Credentials = new NetworkCredential(username, password);

                // Add the payload
                using (var s = wr.GetRequestStream()) {
                    s.Write(contents, 0, contents.Length);
                }

                // Transmit
                var resp = wr.GetResponse();
                using (var s2 = resp.GetResponseStream()) {
                    using (var sr = new StreamReader(s2)) {
                        string message = sr.ReadToEnd();
                        System.Diagnostics.Debug.WriteLine(message);
                        return true;
                    }
                }

                // Catch any problems
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
