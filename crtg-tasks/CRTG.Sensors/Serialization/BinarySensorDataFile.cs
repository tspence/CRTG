/*
 * 2012 - 2015 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/CRTG
 * 
 * This program uses icons from http://www.famfamfam.com/lab/icons/silk/
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CRTG.Common;

namespace CRTG.Serialization
{
    public class BinarySensorDataFile : BaseSensorDataFile
    {
        public BinarySensorDataFile(string filename)
            : base(filename)
        { 
        }

        public override void Load()
        {
            using (var f = File.Open(_filename, FileMode.Open)) {
                using (var b = new BinaryReader(f)) {

                    // Get the version number token
                    int version = b.ReadInt32();

                    // Get all records while the file has more data - this data is appended endlessly
                    try {
                        while (f.Position < f.Length) {
                            _data.Add(ReadRecord(b));
                        }
                    } catch (Exception ex) {
                        SensorProject.LogException("BinarySensorDataFile.Load", ex);
                    }
                    b.Close();
                }
                f.Close();
            }
            _dirty = false;
        }

        public override void Save()
        {
            using (var f = File.Open(_filename, FileMode.Create)) {
                using (var b = new BinaryWriter(f)) {

                    // Write the version number token
                    int version = 1;
                    b.Write(version);

                    // Write all records to disk as they stand right now
                    var array = _data.ToArray();
                    foreach (var r in array) {
                        WriteRecord(b, r);
                    }
                    b.Close();
                }
                f.Close();
            }
            _dirty = false;
        }

        public override void Append(SensorData data)
        {
            // Add to the internal array
            _data.Add(data);

            // Save to disk
            using (var f = File.Open(_filename, FileMode.Append)) {
                using (var b = new BinaryWriter(f)) {
                    WriteRecord(b, data);
                    b.Close();
                }
                f.Close();
            }

            // We're good
            _dirty = false;
        }

        private void WriteRecord(BinaryWriter b, SensorData r)
        {
            b.Write(r.Time.ToBinary());
            b.Write(r.Value);
        }

        private SensorData ReadRecord(BinaryReader b)
        {
            SensorData r = new SensorData();
            r.Time = DateTime.FromBinary(b.ReadInt64());
            r.Value = b.ReadDecimal();
            return r;
        }
    }
}
