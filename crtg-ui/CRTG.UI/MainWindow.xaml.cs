using CRTG.Common;
using CRTG.Common.Interfaces;
using CRTG.Sensors.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CRTG.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SensorViewModel ViewModel { get; set; }

        public MainWindow()
        {
            // Start up our view model
            ViewModel = new SensorViewModel();

            // Load the UI
            InitializeComponent();
        }

        #region Convenience Properties
        /// <summary>
        /// Identify currently selected device, if any
        /// </summary>
        public IDevice SelectedDevice
        {
            get
            {
                return tvSensors1.SelectedItem as IDevice;
            }
        }

        /// <summary>
        /// Identify currently selected sensor, if any
        /// </summary>
        public ISensor SelectedSensor
        {
            get
            {
                return tvSensors1.SelectedItem as ISensor;
            }
        }
        #endregion

        #region Context Menu
        private void tvSensors1_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var parent = sender as TreeView;
            if (parent != null) {
                var item = parent.SelectedItem;
                ContextMenu mnu = null;
                if (item is SensorProject) {
                    mnu = (ContextMenu)this.Resources["mnuProject"];
                } else if (item is SensorDevice) {
                    mnu = (ContextMenu)this.Resources["mnuDevice"];
                } else if (item is BaseSensor) {
                    mnu = (ContextMenu)this.Resources["mnuSensor"];
                }
                if (mnu != null) {
                    mnu.IsOpen = true;
                }
            }
        }

        private void tvSensors1_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null) {
                treeViewItem.Focus();
            }
        }

        private static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        #endregion

        #region Context Menu Items
        private void mnuProject_AddSensor_Click(object sender, EventArgs e)
        {
            var d = SelectedDevice;
            if (d != null) {

                // Pop up a window to design a new sensor
                AddSensor dlg = new AddSensor();
                var result = dlg.ShowDialog();
                if (result.HasValue && result.Value) {

                    // Add this sensor to the currently selected device
                    SensorProject.Current.AddSensor(d, dlg.SensorToAdd);
                }
            }
        }

        private void mnuProject_AddDevice_Click(object sender, RoutedEventArgs e)
        {
            SensorDevice dc = new SensorDevice();
            dc.DeviceName = "New Device";
            SensorProject.Current.AddDevice(dc);
        }

        private void mnuProject_RemoveDevice_Click(object sender, RoutedEventArgs e)
        {
            SensorProject.Current.RemoveChild(SelectedDevice);
        }
        #endregion
    }
}
