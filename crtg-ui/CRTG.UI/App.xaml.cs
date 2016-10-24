using CRTG.Notification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CRTG.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string _filename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "sensors.crtg");
        private string _temp_filename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "sensors_temp.crtg");
        private string _old_filename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "sensors.old.crtg");

        protected override void OnStartup(StartupEventArgs e)
        {
            // Begin sensor execution
            if (File.Exists(_filename)) {
                SensorProject.Current = SensorProject.Deserialize(_filename);
            } else {
                SensorProject.Current = new SensorProject();
            }
            SensorProject.Current.PropertyChanged += SensorProjectChanged;
            SensorProject.Current.Start();

            // Resume launching
            base.OnStartup(e);
        }

        private void SensorProjectChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Only one save at a time, to avoid conflicting writes
            lock (this) {
                try {
                    // Serialize to a temporary file, to avoid losing data if we crash in the middle of this
                    if (File.Exists(_temp_filename)) File.Delete(_temp_filename);
                    SensorProject.Current.Serialize(_temp_filename);

                    // Rename file
                    if (File.Exists(_filename)) {
                        File.Move(_filename, _old_filename);
                    }
                    File.Move(_temp_filename, _filename);

                // If anything blew up, keep the old file
                } catch (Exception ex) {
                    SensorProject.LogException("Serialization", ex);
                    if (File.Exists(_old_filename)) {
                        File.Move(_old_filename, _filename);
                    }

                // Remove the old filename if it's still there
                } finally {
                    if (File.Exists(_old_filename)) {
                        File.Delete(_old_filename);
                    }
                }
            }
        }
    }
}
