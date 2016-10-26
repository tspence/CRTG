/*
 * 2012 - 2016 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/CRTG
 * 
 * This program uses icons from http://www.famfamfam.com/lab/icons/silk/
 */
using System;
using CRTG.Common;
using Newtonsoft.Json;
using CRTG.Common.Interfaces;
using CRTG.Common.Attributes;
using CRTG.Common.Data;

namespace CRTG.Sensors
{
    public class BaseSensor : BaseSensorTreeModel, ISensor
    {
        #region Icon
        /// <summary>
        /// This is the icon path for this object when it is in normal operation.
        /// Override this to show a more friendly icon.
        /// </summary>
        public virtual string GetNormalIconPath()
        {
            return "Resources/sensor.png";
        }

        /// <summary>
        /// The icon that should be displayed for this sensor
        /// </summary>
        public sealed override string IconPath
        {
            get
            {
                // Flicker the icon when we are collecting
                if (InFlight) {
                    return "Resources/hourglass.png";

                // Show paused icons
                } else if (!Enabled) {
                    return "Resources/control_pause_blue.png";
                
                // Show sensors in error
                } else if (InError) {
                    return "Resources/sensor_error.png";

                // Sensor is in normal state
                } else {
                    return GetNormalIconPath();
                }
            }
        }
        #endregion


        #region Properties
        /// <summary>
        /// Longer friendly description for this sensor
        /// </summary>
        [AutoUI(Group = "Sensor")]
        public string Description { get; set; }

        /// <summary>
        /// What unit are we measuring?
        /// </summary>
        [AutoUI(Group = "Sensor")]
        public string MeasurementUnit { get; set; }

        /// <summary>
        /// How often are we measuring it?
        /// </summary>
        [AutoUI(Group = "Sensor")]
        public Interval Frequency { get; set; }

        /// <summary>
        /// How often are we measuring it?
        /// </summary>
        [AutoUI(Group = "Sensor")]
        public bool PauseOnError { get; set; }

        /// <summary>
        /// Is this sensor enabled?
        /// </summary>
        [AutoUI(Skip=true)]
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                Notify("IconPath");
            }
        }

        private bool _enabled;

        /// <summary>
        /// All the data collected with this sensor over time
        /// </summary>
        [JsonIgnore, AutoUI(Skip = true)]
        public SensorDataCollection SensorData { get; set; }

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
        [JsonIgnore, AutoUI(Skip = true)]
        public bool InFlight
        {
            get
            {
                return _inFlight;
            }
            set
            {
                _inFlight = value;
                Notify("IconPath");
            }
        }

        private bool _inFlight;

        /// <summary>
        /// Shortcut to identify the parent device of this sensor
        /// </summary>
        [JsonIgnore]
        public IDevice Device
        {
            get
            {
                return Parent as IDevice;
            }
        }

        /// <summary>
        /// Track whether this collector is erroring out
        /// </summary>
        [AutoUI(Skip=true)]
        public bool InError
        {
            get
            {
                return _inError;
            }
            set
            {
                _inError = value;
                Notify("IconPath");
            }
        }
        private bool _inError;

        /// <summary>
        /// Track whether this collector is erroring out
        /// </summary>
        [AutoUI(Skip=true)]
        public string LastException { get; set; }
        #endregion


        #region Helper functions for data collection
        /// <summary>
        /// Add this value to the time series data and advance our next collection time
        /// </summary>
        /// <param name="d"></param>
        public SensorData AddValue(decimal d, DateTime timestamp, int ms)
        {
            SensorData sd = new SensorData() { Time = timestamp, Value = d, CollectionTimeMs = ms };
            if (SensorData == null) {
                DataRead();
            }
            SensorProject.Current.DataStore.AppendData(this, sd);
            LatestData = d;
            return sd;
        }
        #endregion


        #region Collection and notification wrappers
        /// <summary>
        /// Attach to this event handler to be notified when this sensor collects data
        /// </summary>
        public event SensorCollectEventHandler SensorCollect;

        /// <summary>
        /// Collect data for this sensor (with parameter - not used!)
        /// </summary>
        public void OuterCollect()
        {
            DateTime collectStartTime = DateTime.MinValue;
            DateTime collectFinishTime = DateTime.MinValue;
            bool success = false;
            TimeSpan ts = new TimeSpan();
            SensorData sd = null;
            SensorCollectEventArgs args = new SensorCollectEventArgs();

            // Collect data and clock how long it took
            try {
                collectStartTime = DateTime.UtcNow;
                args.Raw = Collect();
                collectFinishTime = DateTime.UtcNow;
                ts = collectFinishTime - collectStartTime;

                // Record what was collected
                LastCollectTime = collectFinishTime;
                sd = AddValue(args.Raw.Value, collectStartTime, (int)ts.TotalMilliseconds);
                success = true;
                
            // If something blew up, keep track of it
            } catch (Exception ex) {
                args.Exception = new SensorException()
                {
                    Cleared = false,
                    Description = ex.Message,
                    ExceptionTime = DateTime.UtcNow,
                    StackTrace = ex.StackTrace
                };
                LastException = ex.ToString();
                InError = true;
                if (PauseOnError) {
                    Enabled = false;
                    SensorProject.LogException("Sensor error & pause: (#" + this.Identity + ") " + this.Name, ex);
                } else {
                    SensorProject.LogException("Sensor error: (#" + this.Identity + ") " + this.Name, ex);
                }

            // Release the inflight status so that this sensor can collect again
            } finally {

                // Move forward to next collection time period - skip any number of intermediate time periods
                if (NextCollectTime < DateTime.UtcNow) {
                    NextCollectTime = DateTime.UtcNow.AddSeconds((int)Frequency);
                }

                // Allow this to be recollected again
                InFlight = false;
            }

            // Now, all elements that can safely be moved after the inflight flag is turned off
            if (success) {
                args.Data = new Common.SensorData()
                {
                    CollectionTimeMs = (int)ts.TotalMilliseconds,
                    Time = collectFinishTime,
                    Value = args.Raw.Value
                };

                // Is anyone listening?
                if (this.SensorCollect != null) SensorCollect(this, args);
            }

            // Notify everyone of what happened
            if (Children != null) {
                foreach (ICondition c in Children) {
                    c.TestCondition(args);
                }
            }
        }
        #endregion


        #region Functions to override
        /// <summary>
        /// Collect data for this sensor
        /// </summary>
        public virtual CollectResult Collect()
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Reading and writing the raw sensor data
        /// <summary>
        /// Read all data from disk
        /// </summary>
        public void DataRead()
        {
            SensorData = SensorProject.Current.DataStore.RetrieveData(this);

            // Set most recent collect time, if one is available
            var most_recent_record = SensorData.GetLastData();
            if (most_recent_record != null) {
                LastCollectTime = most_recent_record.Time;
            } else {
                LastCollectTime = DateTime.MinValue;
            }

            // Determine last exception message
            var ex = SensorData.GetLastException();
            if (ex != null && !ex.Cleared) {
                LastException = ex.Description;
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        public void ClearAllData()
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Duplicating a sensor
        /// <summary>
        /// Duplicate this sensor
        /// </summary>
        /// <returns></returns>
        public BaseSensor Duplicate()
        {
            var serialized = JsonConvert.SerializeObject(this);
            var s = JsonConvert.DeserializeObject(serialized) as BaseSensor;

            // Set some sensible defaults
            s.Enabled = false;
            s.LastCollectTime = DateTime.MinValue;
            s.LatestData = 0.0m;
            s.NextCollectTime = DateTime.MinValue;
            s.Identity = -1;
            s.Name = s.Name + " (Copy)";

            // Here's your dupe
            return s;
        }
        #endregion
    }
}
