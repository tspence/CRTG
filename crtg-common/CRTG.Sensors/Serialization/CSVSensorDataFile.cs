using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CSVFile;
using CRTG.Sensors.Data;

namespace CRTG.Serialization
{
    public class CSVSensorDataFile : BaseSensorDataFile
    {
        public CSVSensorDataFile(string filename)
            : base(filename)
        { 
        }

        public override void Load()
        {
            if (File.Exists(_filename)) {
                _data = CSV.LoadArray<SensorData>(_filename);
            } else {
                _data = new List<SensorData>();
            }
        }

        public override void Save()
        {
            if (_data != null) {
                if (File.Exists(_filename)) {
                    File.Delete(_filename);
                }

                // Convert to an array to avoid list conflicts
                var array = _data.ToArray();
                array.WriteToStream(_filename, true);
            }
        }

        public override void Append(SensorData data)
        {
            _data.Add(data);
            string csv = CSV.SaveArray<SensorData>(new SensorData[] { data }, false, false);
            File.AppendAllText(_filename, csv);
        }
    }
}
