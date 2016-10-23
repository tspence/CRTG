using CRTG.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Common.Data
{
    public class SensorCollectEventArgs : EventArgs
    {
        /// <summary>
        /// The data that was collected from the sensor.  May be null if the sensor detects an exception.
        /// </summary>
        public SensorData Data { get; set; }

        /// <summary>
        /// If the sensor detects an exception
        /// </summary>
        public SensorException Exception { get; set; }

        /// <summary>
        /// This value is set by the condition when it is tested
        /// </summary>
        public string ConditionMessage { get; set; }

        /// <summary>
        /// If a report is included in the collection, this value is set.
        /// </summary>
        public DataTable Report { get; set; }
    }

    public delegate void SensorCollectEventHandler(ISensor sender, SensorCollectEventArgs e);
}
