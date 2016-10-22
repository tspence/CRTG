using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Common
{
    public class SensorDataCollection
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<SensorData> Data { get; set; }
        public List<SensorException> Exceptions { get; set; }

        #region Constructor
        public SensorDataCollection()
        {
            Data = new List<SensorData>();
            Exceptions = new List<SensorException>();
            StartDate = DateTime.MinValue;
            EndDate = DateTime.MinValue;
        }
        #endregion

        #region Public Interface
        public SensorData GetLastData()
        {
            // Note that the "Data" object is always ordered
            if (Data.Count > 0) return Data[Data.Count - 1];
            return null;
        }

        public SensorException GetLastException()
        {
            // Note that the "Exceptions" object is always ordered
            if (Exceptions.Count > 0) return Exceptions[Exceptions.Count - 1];
            return null;
        }
        #endregion
    }
}
