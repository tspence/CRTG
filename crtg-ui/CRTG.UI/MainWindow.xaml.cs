using CRTG.Charts;
using CRTG.Common;
using CRTG.Common.Interfaces;
using CRTG.Sensors.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
            tvSensors1.SelectedItemChanged += TvSensors1_SelectedItemChanged;
            foreach (var vt in Enum.GetValues(typeof(ViewTimeframe))) {
                this.ddlViewTimeframe.Items.Add(vt);
            }
            this.ddlViewTimeframe.SelectedValue = ViewTimeframe.Day;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ChartImage") {
                var bmp = ViewModel.Chart.ChartImage;
                System.Diagnostics.Debug.WriteLine("Refreshing Image: {0}x{1}", bmp.Width, bmp.Height);
                if (bmp != null) {
                    MemoryStream ms = new MemoryStream();
                    bmp.Save(ms, ImageFormat.Bmp);
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.EndInit();
                    this.autoChart.Source = bi;
                } else {
                    this.autoChart.Source = null;
                }
            }
        }

        private void TvSensors1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.autoProperties.DisplayObject = tvSensors1.SelectedValue;

            // Put up a chart
            var sensor = tvSensors1.SelectedValue as ISensor;
            if (sensor != null) {
                ViewModel.Chart.Sensor = sensor;
            }
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

        #region Tree View
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

        private void mnuProject_Pause_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSensor != null) {
                SelectedSensor.Enabled = !SelectedSensor.Enabled;
            }
        }

        private void mnuProject_ResetError_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSensor != null) {
                SelectedSensor.InError = false;
            }
        }

        private void mnuProject_EmptyHistory_Click(object sender, RoutedEventArgs e)
        {
        }

        private void mnuProject_Duplicate_Click(object sender, RoutedEventArgs e)
        {
        }

        private void mnuProject_RemoveSensor_Click(object sender, RoutedEventArgs e)
        {
        }
        #endregion

        #region Charting
        private void grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.Chart.SetSize((int)((FrameworkElement)sender).ActualWidth, (int)((FrameworkElement)sender).ActualHeight);
        }
        #endregion

        #region Closing
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SensorProject.Current.Stop();
        }
        #endregion
    }
}
