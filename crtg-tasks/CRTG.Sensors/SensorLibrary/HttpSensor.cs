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
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using CRTG.Common;

namespace CRTG.Sensors.SensorLibrary
{
    [SensorUI(Category = "Web", Tooltip = "Request a specific page via HTTP and measure information about that request.")]
    public class HttpSensor : BaseSensor
    {
        public override string GetNormalIconPath()
        {
            return "Resources/html.png";
        }

        [AutoUI(Group = "HTTP")]
        public string URL;

        [AutoUI(Group = "HTTP")]
        public RequestMethod HttpMethod = RequestMethod.GET;

        [AutoUI(Group = "HTTP")]
        public string ContentType;

        [AutoUI(Group = "HTTP")]
        public string Content;

        [AutoUI(Group="HTTP")]
        public string Accept;

        [AutoUI(Group = "Test")]
        public string RegexTest;

        [AutoUI(Group = "Test")]
        public decimal ValueIfSuccess;

        [AutoUI(Group = "Test")]
        public decimal ValueIfFailure;

        #region Implementation
        /// <summary>
        /// Check the HTTP 
        /// </summary>
        /// <returns></returns>
        public override decimal Collect()
        {
            try {

                // Construct the request exactly as the options specify
                HttpWebRequest r = (HttpWebRequest)WebRequest.Create(URL);
                r.Method = HttpMethod.ToString();
                r.ContentType = ContentType;
                r.Accept = Accept;

                // Do we have content?
                if (!String.IsNullOrEmpty(Content)) {
                    byte[] b = Encoding.UTF8.GetBytes(Content);
                    r.ContentLength = b.Length;
                    using (Stream s = r.GetRequestStream()) {
                        s.Write(b, 0, b.Length);
                        s.Close();
                    }
                }

                // Can we get a response?
                int before = Environment.TickCount;
                HttpWebResponse resp = (HttpWebResponse)r.GetResponse();
                if (resp == null) {
                    return ValueIfFailure;
                }

                // Is the response an error?
                if (resp.StatusCode != HttpStatusCode.OK) {
                    return ValueIfFailure;
                }

                // Do we have a test we can apply to the result?
                if (!String.IsNullOrEmpty(RegexTest)) {

                    // Read in result
                    string result = null;
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream())) {
                        result = sr.ReadToEnd();
                    }

                    // Run a regex test
                    Regex rx = new Regex(RegexTest);
                    Match m = rx.Match(result);
                    if (m.Success) {
                        return ValueIfSuccess;
                    } else {
                        return ValueIfFailure;
                    }
                }

                // Everything has passed
                return ValueIfSuccess;

            // Exceptions fail
            } catch (Exception x) {
                SensorProject.LogException("HttpSensor", x);
                return ValueIfFailure;
            }
        }
        #endregion
    }
}
