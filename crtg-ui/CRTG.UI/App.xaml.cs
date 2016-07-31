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

        protected override void OnStartup(StartupEventArgs e)
        {
            // Begin sensor execution
            if (File.Exists(_filename)) {
                SensorProject.Current = SensorProject.Deserialize(_filename);
            } else {
                SensorProject.Current = new SensorProject();
            }
            SensorProject.Current.Notifications = new NotificationHelper();

            // Resume launching
            base.OnStartup(e);
        }
    }
}
