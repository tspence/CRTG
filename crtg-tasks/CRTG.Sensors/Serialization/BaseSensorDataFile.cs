﻿/*
 * 2012 - 2016 Ted Spence, http://tedspence.com
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
    public class BaseSensorDataFile
    {
        protected string _filename;
        protected List<SensorData> _data;
        protected bool _dirty, _is_loaded;


        public BaseSensorDataFile(string filename)
        {
            _filename = filename;
            _data = new List<SensorData>();
            _is_loaded = false;
        }

        public virtual void Load()
        {
            throw new Exception("Abstract Base Class");
        }

        public virtual void Save()
        {
            throw new Exception("Abstract Base Class");
        }

        public virtual void Append(SensorData data)
        {
            _data.Add(data);
            _dirty = true;
        }

        /// <summary>
        /// An array containing all data in the sensor data file
        /// </summary>
        public virtual SensorData[] Data
        {
            get 
            {
                // Lazy loading
                if (!_is_loaded) Load();

                // Here's your data
                return _data.ToArray();
            }
            set
            {
                _data = new List<SensorData>(value);
            }
        }

        /// <summary>
        /// Backup the previous data file, clear it, and start over
        /// </summary>
        public virtual void BackupAndClear()
        {
            // Move the existing file to a backup
            string backup_fn = "backup." + DateTime.UtcNow.ToString("yyyy-MM-dd") + "." + Path.GetFileName(_filename);
            int n = 1;
            while (File.Exists(backup_fn)) {
                n++;
                backup_fn = "backup." + DateTime.UtcNow.ToString("yyyy-MM-dd") + "_" + n + "." + Path.GetFileName(_filename);
            }
            File.Move(_filename, backup_fn);

            // Clear all data in the existing sensor
            _data = new List<SensorData>();
        }

        /// <summary>
        /// How many records are in this file?
        /// </summary>
        public int Count 
        { 
            get
            {
                return _data.Count();
            } 
        }
    }
}
