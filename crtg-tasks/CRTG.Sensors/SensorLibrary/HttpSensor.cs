/*
 * 2012 - 2016 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/CRTG
 * 
 * This program uses icons from http://www.famfamfam.com/lab/icons/silk/
 */
using System;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using CRTG.Common;
using CRTG.Common.Attributes;
using CRTG.Common.Data;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace CRTG.Sensors.SensorLibrary
{
    [SensorUI(Category = "Web", Tooltip = "Request a specific page via HTTP and measure information about that request.")]
    public class HttpSensor : BaseSensor
    {
        [AutoUI(Group = "HTTP")]
        public string URL { get; set; }

        [AutoUI(Group = "HTTP")]
        public HttpVerb HttpMethod { get; set; }

        [AutoUI(Group = "HTTP", MultiLine = 5, Help = "Enter each header on a separate line, with a name, followed by a space, followed by the value.")]
        public string Headers { get; set; }

        [AutoUI(Group = "HTTP", MultiLine = 10)]
        public string RequestBody { get; set; }

        [AutoUI(Group = "Measurement")]
        public HttpMeasurement Measurement { get; set; }

        [AutoUI(Group = "Measurement", Help = "Enter the JSON path, XML path, or regex to identify a decimal value.")]
        public string Expression { get; set; }

        #region Implementation
        /// <summary>
        /// Check the HTTP 
        /// </summary>
        /// <returns></returns>
        public override async Task<CollectResult> Collect()
        {
            using (var client = new HttpClient()) {

                // Setup headers
                if (!String.IsNullOrWhiteSpace(Headers)) {
                    client.DefaultRequestHeaders.Clear();
                    foreach (var s in Headers.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)) {
                        var p = s.IndexOf(' ');
                        if (p > 0) {
                            var name = s.Substring(0, p);
                            var val = s.Substring(p + 1);
                            client.DefaultRequestHeaders.Add(name, val);
                        }
                    }
                }

                // Send the request
                HttpResponseMessage r = null;
                DateTime start = DateTime.UtcNow;
                switch (HttpMethod) {
                    case HttpVerb.DELETE: r = await client.DeleteAsync(URL); break;
                    case HttpVerb.GET: r = await client.GetAsync(URL); break;
                    case HttpVerb.POST: r = await client.PostAsync(URL, new StringContent(RequestBody)); break;
                    case HttpVerb.PUT: r = await client.PutAsync(URL, new StringContent(RequestBody)); break;
                }

                // Parse the response
                string result = await r.Content.ReadAsStringAsync();
                var ts = DateTime.UtcNow - start;

                // Now parse the response
                switch (Measurement) {
                    case HttpMeasurement.RequestTime:
                        return new CollectResult((int)ts.TotalMilliseconds);
                    case HttpMeasurement.BytesReceived:
                        return new CollectResult((int)(Encoding.UTF8.GetBytes(result).Length));
                    case HttpMeasurement.JsonValue:
                        var obj = JObject.Parse(result);
                        var s = obj.SelectToken(Expression).ToString();
                        return new CollectResult(Decimal.Parse(s));
                    case HttpMeasurement.RegexValue:
                        var regex = new Regex(Expression);
                        var m = regex.Match(result);
                        return new CollectResult(Decimal.Parse(m.Groups[0].Value));
                    case HttpMeasurement.RegexMatches:
                        var r2 = new Regex(Expression);
                        var m2 = r2.Matches(result);
                        return new CollectResult(m2.Count);
                    case HttpMeasurement.XmlPath:
                        XmlDocument d = new XmlDocument();
                        d.LoadXml(result);
                        var n = d.SelectSingleNode(Expression);
                        string val = n.InnerText;
                        return new CollectResult(Decimal.Parse(val));
                }
            }

            // Nothing was captured
            throw new NotImplementedException();
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
