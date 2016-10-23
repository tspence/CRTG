using CRTG.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Common.Data
{
    public class SensorCollectEventArgs : EventArgs
    {
        public SensorData Data { get; set; }
        public SensorException Exception { get; set; }
    }

    public delegate void SensorCollectEventHandler(ISensor sender, SensorCollectEventArgs e);
}
