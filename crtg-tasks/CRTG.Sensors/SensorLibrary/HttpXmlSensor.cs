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
using System.Xml;
using CRTG.Common;
using CRTG.Common.Attributes;
using CRTG.Common.Data;

namespace CRTG.Sensors.SensorLibrary
{
    [SensorUI(Category = "Web", Tooltip = "Request a specific page via HTTP and find an object within that page via XML XPath.")]
    public class HttpXmlSensor : BaseSensor
    {
        [AutoUI(Group = "HTTP")]
        public string URL { get; set; }

        [AutoUI(Group = "HTTP")]
        public RequestMethod HttpMethod { get; set; }

        [AutoUI(Group = "HTTP")]
        public string ContentType { get; set; }

        [AutoUI(Group = "HTTP")]
        public string Content { get; set; }

        [AutoUI(Group="HTTP")]
        public string Accept { get; set; }

        [AutoUI(Group = "Xml")]
        public string XPath { get; set; }

        [AutoUI(Group = "Xml")]
        public XmlObjectMeasurement Measurement { get; set; }

        [AutoUI(Group = "Test")]
        public decimal ValueIfSuccess { get; set; }

        [AutoUI(Group = "Test")]
        public decimal ValueIfFailure { get; set; }


        #region Implementation
        /// <summary>
        /// Check the HTTP 
        /// </summary>
        /// <returns></returns>
        public override CollectResult Collect()
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
                    return new CollectResult(ValueIfFailure);
                }

                // Is the response an error?
                if (resp.StatusCode != HttpStatusCode.OK) {
                    return new CollectResult(ValueIfFailure);
                }

                // Read in result
                string result = null;
                using (StreamReader sr = new StreamReader(resp.GetResponseStream())) {
                    result = sr.ReadToEnd();
                }

                // Convert this to an Xml object and retrieve the selected node
                XmlDocument d = new XmlDocument();
                d.LoadXml(result);
                var n = d.SelectSingleNode(XPath);
                string val = n.InnerText;

                // Now parse it based on the measurement
                switch (this.Measurement) {
                    case XmlObjectMeasurement.DateDiffFromLocal:
                        DateTime dt_local = DateTime.Parse(val);
                        TimeSpan ts_local = DateTime.UtcNow - dt_local;
                        return new CollectResult((decimal)ts_local.TotalSeconds);
                    case XmlObjectMeasurement.DateDiffFromUTC:
                        DateTime dt_utc = DateTime.Parse(val);
                        TimeSpan ts_utc = DateTime.UtcNow - dt_utc;
                        return new CollectResult((decimal)ts_utc.TotalSeconds);
                    case XmlObjectMeasurement.Decimal:
                        return new CollectResult(Decimal.Parse(val));
                }

                // Fallthrough!
                return new CollectResult(0);

            // Exceptions fail
            } catch (Exception x) {
                SensorProject.LogException("HttpXmlSensor", x);
                return new CollectResult(ValueIfFailure);
            }
        }
        #endregion

        #region Icon
        public override string GetNormalIconPath()
        {
            return "Resources/html.png";
        }
        #endregion
    }
}
