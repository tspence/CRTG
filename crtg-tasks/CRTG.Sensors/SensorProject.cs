﻿/*
 * 2012 - 2016 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/CRTG
 * 
 * This program uses icons from http://www.famfamfam.com/lab/icons/silk/
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Serialization;
using System.IO;
using log4net;
using CRTG.Common;
using CRTG.Sensors.Data;
using Newtonsoft.Json;
using CRTG.Common.Interfaces;
using CRTG.Common.Attributes;
using System.Threading.Tasks;

namespace CRTG.Sensors
{
    [Serializable]
    public class SensorProject : BaseSensorTreeModel
    {
        /// <summary>
        /// The hostname or IP address of the SMTP server
        /// </summary>
        [AutoUI(Group = "SMTP")]
        public string SmtpHost { get; set; }

        /// <summary>
        /// username to provide to the SMTP server
        /// </summary>
        [AutoUI(Group = "SMTP")]
        public string SmtpUsername { get; set; }

        /// <summary>
        /// username to provide to the SMTP server
        /// </summary>
        [AutoUI(Group = "SMTP", PasswordField = true)]
        public string SmtpPassword { get; set; }

        /// <summary>
        /// Does the user want local time or GMT?
        /// </summary>
        [AutoUI(Group = "Preferences")]
        public DateTimePreference TimeZonePreference { get; set; }

        /// <summary>
        /// Does the user want flat CSV files, or full OpenXML?
        /// </summary>
        [AutoUI(Group = "Preferences")]
        public ReportFileFormat ReportFileFormatPreference { get; set; }

        /// <summary>
        /// The "from" name to use for the email message
        /// </summary>
        [AutoUI(Group = "SMTP")]
        public string MessageFrom { get; set; }

        /// <summary>
        /// Credentials to use for Klipfolio
        /// </summary>
        [AutoUI(Label = "Klipfolio Username", Group = "Klipfolio")]
        public string KlipfolioUsername { get; set; }

        /// <summary>
        /// Credentials to use for Klipfolio
        /// </summary>
        [AutoUI(Label = "Klipfolio Password", Group = "Klipfolio", PasswordField = true)]
        public string KlipfolioPassword { get; set; }

        /// <summary>
        /// Indicates the method we use to store data
        /// </summary>
        [AutoUI(Skip = true), JsonIgnore]
        public IDataStore DataStore { get; set; }


        #region Observables
        /// <summary>
        /// The icon that should be displayed for this sensor
        /// </summary>
        public override string IconPath
        {
            get
            {
                return "Resources/project.png";
            }
        }
        #endregion


        #region Logging
        private static ILog _logger = null;

        /// <summary>
        /// The log to use for outputting debug information
        /// </summary>
        [AutoUI(Skip = true)]
        private static ILog Log
        {
            get
            {
                // Make sure we have a logging object we can use
                if (_logger == null) {
                    log4net.Config.XmlConfigurator.Configure();
                    _logger = (ILog)log4net.LogManager.GetLogger(typeof(SensorProject));
                }

                // Here's the logger
                return _logger;
            }
        }
        #endregion


        #region Managing the collection of sensors and devices
        /// <summary>
        /// Adds a new device to the project
        /// </summary>
        /// <param name="deviceToAdd"></param>
        public void AddDevice(IDevice deviceToAdd)
        {
            deviceToAdd.Identity = GetNextDeviceNum();
            AddChild(deviceToAdd);
        }

        /// <summary>
        /// Adds a new sensor to a device
        /// </summary>
        /// <param name="device"></param>
        public void AddSensor(IDevice device, ISensor sensorToAdd)
        {
            sensorToAdd.Identity = GetNextSensorNum();
            device.AddChild(sensorToAdd);
        }

        /// <summary>
        /// Add a condition to a sensor
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="sensorToAdd"></param>
        public void AddCondition(ISensor sensor, ICondition conditionToAdd)
        {
            conditionToAdd.Identity = GetNextConditionNum();
            sensor.AddChild(conditionToAdd);
        }

        /// <summary>
        /// Add an action to a condition
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="sensorToAdd"></param>
        public void AddAction(ICondition condition, IAction sensorToAdd)
        {
            sensorToAdd.Identity = GetNextActionNum();
            condition.AddChild(sensorToAdd);
        }

        [AutoUI(Skip = true)]
        public int NextDeviceNum { get; set; }

        [AutoUI(Skip = true)]
        public int NextSensorNum { get; set; }

        [AutoUI(Skip = true)]
        public int NextConditionNum { get; set; }

        [AutoUI(Skip = true)]
        public int NextActionNum { get; set; }

        /// <summary>
        /// Returns next device number in sequence
        /// </summary>
        /// <returns></returns>
        public int GetNextDeviceNum()
        {
            lock (this) {
                return NextDeviceNum++;
            }
        }

        /// <summary>
        /// Returns next sensor number in sequence
        /// </summary>
        /// <returns></returns>
        public int GetNextSensorNum()
        {
            lock (this) {
                return NextSensorNum++;
            }
        }

        /// <summary>
        /// Returns next condition number in sequence
        /// </summary>
        /// <returns></returns>
        public int GetNextConditionNum()
        {
            lock (this) {
                return NextConditionNum++;
            }
        }

        /// <summary>
        /// Returns next sensor number in sequence
        /// </summary>
        /// <returns></returns>
        public int GetNextActionNum()
        {
            lock (this) {
                return NextActionNum++;
            }
        }

        #endregion


        #region Multithreaded data gathering from the sensors
        protected Thread _collection_thread = null;
        protected bool _keep_running = false;

        /// <summary>
        /// Start collection of data for this project
        /// </summary>
        public void Start()
        {
            if (_keep_running == false) {
                _keep_running = true;
                Task.Run(() => CollectionThread());
            }
        }

        /// <summary>
        /// Stop collection of data for this project (after current sensors have fired)
        /// </summary>
        public void Stop()
        {
            if (_collection_thread != null) {
                _keep_running = false;
            }
        }

        /// <summary>
        /// This is the background thread for collecting data
        /// </summary>
        public async void CollectionThread()
        {
            // Okay, let's enter the loop
            while (_keep_running) {
                int collect_count = 0;
                DateTime next_collect_time = DateTime.MaxValue;

                // Be safe about this - we don't want this thread to blow up!  It's the only one we've got
                try {

                    // Loop through sensors, and spawn a work item for them
                    for (int i = 0; i < Children.Count; i++) {
                        IDevice dc = Children[i] as IDevice;
                        for (int j = 0; j < dc.Children.Count; j++) {

                            // Allow us to kick out
                            if (!_keep_running) return;

                            // Okay, let's work on this sensor
                            ISensor s = dc.Children[j] as ISensor;
                            if ((s.Enabled) && (!s.InFlight) && (s.Frequency != Interval.Never)) {

                                // Spawn a work item in the thread pool to do this collection task
                                if (s.NextCollectTime <= DateTime.UtcNow) {
                                    s.InFlight = true;
                                    Task.Run(() => s.OuterCollect());
                                    collect_count++;

                                // If it's not time yet, use this to factor when next to wake up
                                } else {
                                    if (s.NextCollectTime < next_collect_time) {
                                        next_collect_time = s.NextCollectTime;
                                    }
                                }
                            }
                        }
                    }

                // Failsafe
                } catch (Exception ex) {
                    SensorProject.LogException("Collect", ex);
                }

                // Sleep until next collection time, but allow ourselves to kick out
                if (!_keep_running) return;
                TimeSpan time_to_sleep = next_collect_time - DateTime.UtcNow;
                int clean_sleep_time = Math.Max(1, Math.Min((int)time_to_sleep.TotalMilliseconds, 1000));
                await Task.Delay(clean_sleep_time);
            }
        }
        #endregion


        #region Serialization and Singleton
        [AutoUI(Skip = true)]
        public static SensorProject Current = null;

        public SensorProject()
        {
            // Fetch data from the file
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            if (path.StartsWith("file:\\", StringComparison.CurrentCultureIgnoreCase)) {
                path = path.Substring(6);
            }

            // Refactor to move to SQLite
            DataStore = new SqliteDataStore(Path.Combine(path, "sensors.sqlite"));
        }

        /// <summary>
        /// Read a sensor back from a node from an XML file
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static SensorProject Deserialize(string filename)
        {
            SensorProject sp = null;

            // Read in the text
            try {
                var settings = new JsonSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.Objects;
                settings.Formatting = Newtonsoft.Json.Formatting.Indented;
                string s = File.ReadAllText(filename);
                sp = JsonConvert.DeserializeObject<SensorProject>(s, settings);

            // Failed to load
            } catch (Exception ex) {
                SensorProject.LogException("Error loading sensor file", ex);
                sp = new SensorProject();
            }

            // Now make all the sensors read their data
            Current = sp;
            List<int> sensor_id_list = new List<int>();
            foreach (IDevice dc in sp.Children) {
                dc.Parent = SensorProject.Current;

                // Go through all sensors for this device
                foreach (ISensor bs in dc.Children) {

                    // Make sure each sensor is uniquely identified!  If any have duplicate IDs, uniqueify them
                    if (sensor_id_list.Contains(bs.Identity)) {
                        bs.Identity = sp.NextSensorNum++;
                    }
                    sensor_id_list.Add(bs.Identity);
                    bs.Parent = (IDevice)dc;

                    // Read in each sensor's data, and write it back out to disk 
                    // (this ensures that all files have the same fields in the same order - permits appending via AppendText later
                    bs.DataRead();

                    // Loop through all conditions / actions
                    foreach (ICondition c in bs.Children) {
                        c.Parent = bs;
                        foreach (IAction a in c.Children) {
                            a.Parent = c;
                        }
                    }
                }
            }

            // Ensure that the log folder exists
            if (!Directory.Exists("logs")) Directory.CreateDirectory("Logs");

            // Save this as the current project
            return sp;
        }

        static void deserializer_UnreferencedObject(object sender, UnreferencedObjectEventArgs e)
        {
            Log.DebugFormat("Unknown object type [{0}]", e.UnreferencedId);
        }

        static void deserializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            Log.DebugFormat("Unknown attribute [{0}={1}]", e.Attr.Name, e.Attr.Value);
        }

        static void deserializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            Log.DebugFormat("Unknown element [{0}]", e.Element.Name);
        }

        static void deserializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            Log.DebugFormat("Unknown node [{0}]", e.Name);
        }

        /// <summary>
        /// Write this sensor out to a configuration file
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public void Serialize(string filename)
        {
            try {
                var settings = new JsonSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.Objects;
                settings.Formatting = Newtonsoft.Json.Formatting.Indented;
                var s = JsonConvert.SerializeObject(SensorProject.Current, settings);
                File.WriteAllText(filename, s);
            } catch (Exception ex) {
                SensorProject.LogException("Error saving sensor project", ex);
            }
        }
        #endregion


        #region Logging
        public static void LogException(string Location, Exception ex)
        {
            string message = String.Format("CRTG caught an internal exception in [{0}]: {1}", Location, ex.ToString());
            Log.Error(message);
            System.Diagnostics.Debug.WriteLine(message);
        }

        public static void LogMessage(string message)
        {
            Log.Debug(message);
        }
        #endregion
    }
}
