using CRTG.Common.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Common.Interfaces
{
    public interface ISensor : ISensorTreeModel
    {
        string Description { get; set; }
        string MeasurementUnit { get; set; }
        Interval Frequency { get; set; }
        bool PauseOnError { get; set; }
        bool Enabled { get; set; }
        SensorDataCollection SensorData { get; set; }
        Decimal LatestData { get; set; }
        DateTime NextCollectTime { get; set; }
        DateTime LastCollectTime { get; set; }
        bool InFlight { get; set; }
        string LastException { get; set; }
        bool InError { get; set; }

        /// <summary>
        /// Attach to this event to get notifications of new data
        /// </summary>
        SensorCollectEventHandler SensorCollect { get; set; }

        /// <summary>
        /// This function collects data for the sensor
        /// </summary>
        void OuterCollect();

        /// <summary>
        /// This function reads data from the selected data storage system
        /// </summary>
        void DataRead();
    }
}
