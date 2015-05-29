/*
 * 2012 - 2015 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/CRTG
 * 
 * This program uses icons from http://www.famfamfam.com/lab/icons/silk/
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CSVFile;
using System.IO;
using CRTG.Sensors;
using System.Collections.Concurrent;
using CRTG.Serialization;
using CRTG.Sensors.Devices;
using CRTG.Sensors.SensorLibrary;
using CRTG.Sensors.Data;

namespace CRTG
{
    [Serializable, XmlInclude(typeof(SqlSensor)), XmlInclude(typeof(WmiCpuSensor)), XmlInclude(typeof(BasicWmiSensor)), XmlInclude(typeof(HttpSensor)),
    XmlInclude(typeof(SqlReportSensor)), XmlInclude(typeof(WmiDiskSensor)), XmlInclude(typeof(WmiMemorySensor)),
    XmlInclude(typeof(WmiDiskActivity)), XmlInclude(typeof(FileSensor)), XmlInclude(typeof(FolderSensor)), XmlInclude(typeof(HttpXmlSensor)), 
    XmlInclude(typeof(BaseNotificationSystem))]
    public class BaseSensor
    {
        /// <summary>
        /// The sensor's identity
        /// </summary>
        [AutoUI(Skip = true)]
        public int Identity { get; set; }

        /// <summary>
        /// Friendly name for this sensor
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Longer friendly description for this sensor
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// What unit are we measuring?
        /// </summary>
        public string MeasurementUnit { get; set; }

        /// <summary>
        /// How often are we measuring it?
        /// </summary>
        public Interval Frequency { get; set; }

        /// <summary>
        /// How often are we measuring it?
        /// </summary>
        public bool PauseOnError { get; set; }

        /// <summary>
        /// Is this sensor enabled?
        /// </summary>
        [AutoUI(Skip=true)]
        public bool Enabled { get; set; }

        /// <summary>
        /// All the data collected with this sensor over time
        /// </summary>
        [XmlIgnore, AutoUI(Skip = true)]
        public BaseSensorDataFile SensorDataFile { get; set; }

        /// <summary>
        /// The latest available data from the sensor
        /// </summary>
        [AutoUI(Skip = true)]
        public Decimal LatestData { get; set; }

        /// <summary>
        /// Next recommended time to collect data
        /// </summary>
        [AutoUI(Skip = true)]
        public DateTime NextCollectTime { get; set; }

        /// <summary>
        /// Next recommended time to collect data
        /// </summary>
        [AutoUI(Skip = true)]
        public DateTime LastCollectTime { get; set; }

        /// <summary>
        /// Keeps track of whether a collection call is in flight
        /// </summary>
        [XmlIgnore, AutoUI(Skip=true)]
        public bool InFlight = false;

        /// <summary>
        /// Keeps track of whether a collection call is in flight
        /// </summary>
        [XmlIgnore, AutoUI(Skip=true)]
        public DeviceContext Device;

        /// <summary>
        /// Track whether this collector is erroring out
        /// </summary>
        [AutoUI(Skip=true)]
        public bool InError = false;

        /// <summary>
        /// Track whether this collector is erroring out
        /// </summary>
        [AutoUI(Skip=true)]
        public string LastException { get; set; }

        [AutoUI(Group = "Error", Label = "High Threshold")]
        public decimal? HighError;

        [AutoUI(Group = "Error", Label = "Message")]
        public string ErrorMessage;

        [AutoUI(Group = "Error", Label = "Low Threshold")]
        public decimal? LowError;

        [AutoUI(Group = "Warning", Label = "High Threshold")]
        public decimal? HighWarning;

        [AutoUI(Group = "Warning", Label = "Message")]
        public string WarningMessage;

        [AutoUI(Group = "Warning", Label = "Low Threshold")]
        public decimal? LowWarning;

        [AutoUI(Group = "Notifications", Label = "Notify on Change")]
        public bool NotifyOnChange;

        [AutoUI(Group = "Notifications", Label = "Method")]
        public NotificationMethod Method;

        [AutoUI(Group = "Notifications", Label = "Recipients", Help = "A comma-separated list of email addresses.")]
        public string Recipients;

        [AutoUI(Group = "ReportUpload", Help = "(optional) If this report is to be uploaded to a web resource, this is the URL where it is published.")]
        public string UploadUrl;

        [AutoUI(Group = "ReportUpload", Help = "(optional) The username to use when uploading this report to a web resource.")]
        public string UploadUsername;

        [AutoUI(Group = "ReportUpload", Help = "(optional) The password to use when uploading this report to a web resource.")]
        public string UploadPassword;

        [AutoUI(Group = "ReportUpload", Help = "(optional) How frequently to upload this report.")]
        public Interval UploadFrequency;

        [AutoUI(Group = "ReportUpload", Help = "(optional) How much data to upload for this report.")]
        public ViewTimeframe UploadAmount;

        /// <summary>
        /// Keeps track of when we last uploaded this object
        /// </summary>
        [XmlIgnore, AutoUI(Skip = true)]
        public DateTime LastUploadTime;

        #region Helper functions for data collection
        /// <summary>
        /// Add this value to the time series data and advance our next collection time
        /// </summary>
        /// <param name="d"></param>
        public SensorData AddValue(decimal d, DateTime timestamp, int ms)
        {
            SensorData sd = new SensorData() { Time = timestamp, Value = d, CollectionTimeMs = ms };
            if (SensorDataFile == null) {
                DataRead();
            }
            SensorDataFile.Append(sd);
            LatestData = d;
            return sd;
        }
        #endregion


        #region Collection and notification wrappers
        /// <summary>
        /// Collect data for this sensor (with parameter - not used!)
        /// </summary>
        public void OuterCollect(object o)
        {
            DateTime collect_start_time = DateTime.UtcNow;
            Decimal value = 0;
            bool success = false;
            TimeSpan ts = new TimeSpan();
            SensorData sd = null;

            // Collect data and clock how long it took
            try {
                value = Collect();
                ts = DateTime.UtcNow - collect_start_time;
                sd = AddValue(value, collect_start_time, (int)ts.TotalMilliseconds);
                success = true;

            } catch (Exception ex) {
                LastException = ex.ToString();
                InError = true;
                if (PauseOnError) {
                    Enabled = false;
                    SensorProject.Log.ErrorFormat("Sensor paused due to error collecting {0}: {1}", Identity, ex.ToString());
                } else {
                    SensorProject.Log.ErrorFormat("Error collecting {0}: {1}", Identity, ex.ToString());
                }
            }

            // Move forward to next collection time period - skip any number of intermediate time periods
            while (NextCollectTime < DateTime.UtcNow) {
                LastCollectTime = NextCollectTime;
                NextCollectTime = NextCollectTime.AddSeconds((int)Frequency);
            }

            // Test all conditions for this sensor
            InFlight = false;

            // Now, all elements that can safely be moved after the inflight flag is turned off
            if (success) {
                TestAllNotifications(collect_start_time, value);
                UploadCollection();
            }
        }

        protected virtual void UploadCollection()
        {
            if (UploadUrl != null) {
                try {
                    TimeSpan ts = DateTime.UtcNow - LastUploadTime;
                    if ((ts.TotalSeconds > (int)UploadFrequency) && (this.SensorDataFile != null)) {

                        // Filter to just the amount we care about
                        List<SensorData> list = null;
                        if (UploadAmount == ViewTimeframe.AllTime) {
                            list = this.SensorDataFile.Data.ToList();
                        } else {
                            var limit = DateTime.UtcNow.AddMinutes(-(int)UploadAmount);
                            list = (from sd in this.SensorDataFile.Data where (sd != null) && (sd.Time > limit) select sd).ToList();
                        }
                        string csv = list.WriteToString(false);
                        byte[] raw = Encoding.UTF8.GetBytes(csv);
                        ReportUploader.UploadReport(raw, UploadUrl, UploadUsername, UploadPassword);
                        LastUploadTime = DateTime.UtcNow;
                    }

                // Catch problems in uploading
                } catch (Exception ex) {
                    SensorProject.Log.Debug("Error uploading collection to server: " + ex.ToString());
                }
            }
        }

        [XmlIgnore, AutoUI(Skip = true)]
        private decimal? _prior_value;

        [XmlIgnore, AutoUI(Skip = true)]
        private NotificationState _current_state = NotificationState.Normal;

        [XmlIgnore, AutoUI(Skip = true)]
        public NotificationState CurrentState
        {
            get { return _current_state; }
        }

        /// <summary>
        /// Test all notification states and generate any email messages if necessary
        /// </summary>
        private void TestAllNotifications(DateTime timestamp, decimal value)
        {
            // What's our current state?  If this is a state change, notify
            NotificationState ns = TestState(value);
            if (ns != _current_state) {
                string s = "";
                switch (ns) {
                    case NotificationState.ErrorHigh:
                    case NotificationState.ErrorLow:
                        s = ErrorMessage;
                        break;
                    case NotificationState.WarningHigh:
                    case NotificationState.WarningLow:
                        s = WarningMessage;
                        break;
                    case NotificationState.Normal:
                        s = "Sensor returned to normal";
                        break;
                }
                if (SensorProject.Current.Notifications != null) {
                    SensorProject.Current.Notifications.Notify(this, ns, timestamp, value, s);
                }
                _current_state = ns;
            }

            // Check for notification on value changes
            if (this.NotifyOnChange) {
                if ((_prior_value != null) && (_prior_value != value)) {
                    if (SensorProject.Current.Notifications != null) {
                        SensorProject.Current.Notifications.Notify(this, NotificationState.ValueChanged, timestamp, value, String.Format("Changed from {0} to {1}", _prior_value.Value, value));
                    }
                }
                _prior_value = value;
            }
        }

        private NotificationState TestState(decimal value)
        {
            // Test states in order so they don't falsely alarm
            if (HighError != null && value > HighError.Value) {
                return NotificationState.ErrorHigh;
            } else if (HighWarning != null && value > HighWarning.Value) {
                return NotificationState.WarningHigh;
            } else if (LowError != null && value < LowError.Value) {
                return NotificationState.ErrorLow;
            } else if (LowWarning != null && value < LowWarning.Value) {
                return NotificationState.WarningLow;
            }

            // No state
            return NotificationState.Normal;
        }
        #endregion


        #region Functions to override
        /// <summary>
        /// Collect data for this sensor
        /// </summary>
        public virtual decimal Collect()
        {
            throw new Exception("You must override this function.");
        }
        #endregion


        #region Reading and writing the raw sensor data
        /// <summary>
        /// Read all data from disk
        /// </summary>
        public void DataRead()
        {
            SensorDataFile = SensorDataFactory.GetFile(Identity);

            // Set most recent collect time, exception, and value
            if (SensorDataFile.Count > 0) {
                var most_recent_record = SensorDataFile.Data[SensorDataFile.Count - 1];
                this.LastCollectTime = most_recent_record.Time;
                this.LastException = most_recent_record.Exception;
                this.LatestData = most_recent_record.Value;
            }
        }

        public void ClearAllData()
        {
            SensorDataFile.BackupAndClear();
        }
        #endregion


        #region Duplicating a sensor
        /// <summary>
        /// Convert a single sensor to XML
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToXml(BaseSensor s)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BaseSensor));
            using (MemoryStream ms = new MemoryStream()) {

                // Write the XML to a memory object
                var sw = new StreamWriter(ms, Encoding.UTF8);
                serializer.Serialize(sw, s);

                // Rewind
                sw.Flush();
                ms.Position = 0;

                // Read back the XML we produced
                using (var sr = new StreamReader(ms, Encoding.UTF8)) {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Convert a single sensor from XML into an object
        /// </summary>
        /// <param name="xml_string"></param>
        /// <returns></returns>
        public static BaseSensor FromXml(string xml_string)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(BaseSensor));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml_string), false)) {
                using (StreamReader sr = new StreamReader(ms, Encoding.UTF8)) {
                    return (BaseSensor)deserializer.Deserialize(sr);
                }
            }
        }

        /// <summary>
        /// Duplicate this sensor
        /// </summary>
        /// <returns></returns>
        public BaseSensor Duplicate()
        {
            string xml = ToXml(this);
            var s = FromXml(xml);
            s.Enabled = false;
            s.Name = s.Name + " (Copy)";
            return s;
        }
        #endregion
    }
}
