using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace CRTG.Sensors.SensorLibrary
{
    public class HttpXmlSensor : BaseSensor
    {
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

        [AutoUI(Group = "Xml")]
        public string XPath;

        [AutoUI(Group = "Xml")]
        public XmlObjectMeasurement Measurement;

        [AutoUI(Group = "Test")]
        public decimal ValueIfSuccess;

        [AutoUI(Group = "Test")]
        public decimal ValueIfFailure;

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
                        return (decimal)ts_local.TotalSeconds;
                    case XmlObjectMeasurement.DateDiffFromUTC:
                        DateTime dt_utc = DateTime.Parse(val);
                        TimeSpan ts_utc = DateTime.UtcNow - dt_utc;
                        return (decimal)ts_utc.TotalSeconds;
                    case XmlObjectMeasurement.Decimal:
                        return Decimal.Parse(val);
                }

                // Fallthrough!
                return 0;

            // Exceptions fail
            } catch (Exception x) {
                SensorProject.Log.Debug(x.ToString());
                return ValueIfFailure;
            }
        }
    }
}
