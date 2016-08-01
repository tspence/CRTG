using CRTG.Common;
using CRTG.Common.Interfaces;
using CRTG.Sensors.Devices;
using System;
using System.Collections.Generic;
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
        public List<ISensorTreeModel> SensorTree { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // Let's load up the sensor tree!
            SensorTree = new List<ISensorTreeModel>();
            SensorTree.Add(SensorProject.Current);
            SensorProject.Current.Name = "CRTG Project";
            tvSensors1.DataContext = this;
        }

        #region Convenience Properties

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
        private void mnuDevice_AddSensor_Click(object sender, EventArgs e)
        {
            // Figure out the typename of the sensor
            Assembly a = typeof(BaseSensor).Assembly;
            string name = ((MenuItem)sender).Header.ToString();
            var matching_type = (from t in a.GetTypes() where t.Name == name select t).FirstOrDefault();
            if (matching_type != null) {

                // Make an instance of this type
                BaseSensor bs = (BaseSensor)a.CreateInstance(matching_type.FullName, true);
                bs.Name = "New " + bs.GetType().Name;
                bs.Frequency = Interval.FifteenMinutes;
                SensorProject.Current.AddSensor((IDevice)SelectedDevice, bs);
                ((App)(App.Current)).SaveSensors();
            }
        }

        private void mnuProject_AddDevice_Click(object sender, RoutedEventArgs e)
        {
            SensorDevice dc = new SensorDevice();
            dc.DeviceName = "New Device";
            SensorProject.Current.AddDevice(dc);
            ((App)(App.Current)).SaveSensors();
        }
        #endregion
    }
}
