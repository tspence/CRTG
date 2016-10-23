using CRTG.Common;
using CSVFile;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Actions.ActionLibrary
{
    public class HttpUploadNotification : BaseAction
    {
        /// <summary>
        /// Convenience shortcut for uploading a report if you have a list of sensor data or some other thing
        /// </summary>
        /// <param name="list"></param>
        /// <param name="url"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        protected bool UploadReport<T>(List<T> list, bool include_header_row, string url, HttpVerb verb, string username, string password)
        {
            if (list == null) return false;
            string csv = list.WriteToString(include_header_row);
            byte[] raw = Encoding.UTF8.GetBytes(csv);
            return InternalUploadReport(raw, url, verb, username, password);
        }

        /// <summary>
        /// Convenience shortcut for uploading a report if you have a datatable
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="url"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        protected bool UploadReport(DataTable dt, bool include_header_row, string url, HttpVerb verb, string username, string password)
        {
            if (dt == null) return false;
            string csv = dt.WriteToString(include_header_row);
            byte[] raw = Encoding.UTF8.GetBytes(csv);
            return InternalUploadReport(raw, url, verb, username, password);
        }

        /// <summary>
        /// Upload this report to a URL data source on the Internet
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="url"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        protected bool InternalUploadReport(byte[] contents, string url, HttpVerb verb, string username, string password)
        {
            if (String.IsNullOrWhiteSpace(url)) return false;

            // Is this object intended to upload to a web resource?
            try {
                using (var client = new HttpClient()) {

                    // Assemble headers
                    if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password)) {
                        string authInfo = username + ":" + password;
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.Default.GetBytes(authInfo)));
                    }

                    // Send the HTTP message
                    HttpResponseMessage msg = null;
                    if (verb == HttpVerb.POST) {
                        msg = client.PostAsync(url, new ByteArrayContent(contents)).Result;
                    } else if (verb == HttpVerb.PUT) {
                        msg = client.PutAsync(url, new ByteArrayContent(contents)).Result;
                    } else {
                        throw new NotImplementedException();
                    }

                    // Read response and log it
                    var s = msg.Content.ReadAsStringAsync().Result;
                    if (msg.IsSuccessStatusCode) {
                        SensorProject.LogMessage("Uploaded successfully to " + url);
                    } else {
                        SensorProject.LogMessage("Error uploading to " + url + ": " + s);
                    }
                    return true;
                }

            // Catch any problems
            } catch (Exception ex) {
                SensorProject.LogException("Uploading report", ex);
                return false;
            }
        }
    }
}
