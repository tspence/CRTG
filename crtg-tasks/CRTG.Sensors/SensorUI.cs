using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Sensors
{
    public class SensorUI : Attribute
    {
        /// <summary>
        /// Where should this sensor appear in the UI?
        /// </summary>
        public string Category;

        /// <summary>
        /// When 
        /// </summary>
        public string Tooltip;
    }
}
