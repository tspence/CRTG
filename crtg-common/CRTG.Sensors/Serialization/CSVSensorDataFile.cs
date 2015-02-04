using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CSVFile;
using CRTG.Sensors.Data;
using System.Data;

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
                DataTable dt = CSV.LoadDataTable(_filename, false, true, ',', '\"');
                _data = new List<SensorData>();

                // Construct a new CSVReader and be careful about this
                using (var csv = new CSVReader(_filename, ',', '\"', false)) {
                    foreach (string[] line in csv) {
                        if (line != null && line.Length >= 2) {
                            DateTime t = DateTime.MinValue;
                            Decimal val;

                            // Load the exception if present
                            string exception = null;
                            if (line.Length >= 3) exception = line[2];

                            // Load the measurement time if present
                            int ms = 0;
                            if (line.Length >= 4)  int.TryParse(line[3], out ms); 

                            // Save values
                            if (DateTime.TryParse(line[0], out t) && Decimal.TryParse(line[1], out val)) {
                                SensorData sd = new SensorData()
                                {
                                    Time = t,
                                    Value = val,
                                    Exception = exception,
                                    CollectionTimeMs = ms
                                };
                                _data.Add(sd);
                            }
                        }
                    }
                }
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
