/*
 * 2006 - 2012 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://code.google.com/p/csharp-striker/
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRTG.Sensors.Data
{
    public class SensorData
    {
        public DateTime Time { get; set; }
        public decimal Value { get; set; }
        public string Exception { get; set; }
        public int CollectionTimeMs { get; set; }
    }
}
