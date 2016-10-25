using CRTG.Actions;
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
            // Load the UI
            InitializeComponent();

            // Detect the view model
            ViewModel = this.DataContext as SensorViewModel;

            // Okay, now let's proceed
            tvSensors1.SelectedItemChanged += TvSensors1_SelectedItemChanged;
            foreach (var vt in Enum.GetValues(typeof(ViewTimeframe))) {
                this.ddlViewTimeframe.Items.Add(vt);
            }
            this.ddlViewTimeframe.SelectedValue = ViewTimeframe.Day;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        #region Refresh chart image
        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ChartImage") {

                // Track the new bitmap
                var bmp = ViewModel.Chart.ChartImage;
                System.Diagnostics.Debug.WriteLine("Refreshing Image: {0}x{1}", bmp.Width, bmp.Height);

                // Push the change to the UI
                if (!isClosing) {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        BitmapImage bi = null;
                        if (bmp != null) {
                            MemoryStream ms = new MemoryStream();
                            bmp.Save(ms, ImageFormat.Bmp);
                            bi = new BitmapImage();
                            bi.BeginInit();
                            ms.Seek(0, SeekOrigin.Begin);
                            bi.StreamSource = ms;
                            bi.EndInit();
                        }
                        this.autoChart.Source = bi;
                    }));
                }
            }
        }
        #endregion

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

        /// <summary>
        /// Identify currently selected condition, if any
        /// </summary>
        public ICondition SelectedCondition
        {
            get
            {
                return tvSensors1.SelectedItem as ICondition;
            }
        }

        /// <summary>
        /// Identify currently selected action, if any
        /// </summary>
        public IAction SelectedAction
        {
            get
            {
                return tvSensors1.SelectedItem as IAction;
            }
        }
        #endregion

        #region Tree View
        private void TvSensors1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.autoProperties.DisplayObject = tvSensors1.SelectedValue;

            // Put up a chart
            var sensor = tvSensors1.SelectedValue as ISensor;
            if (sensor != null) {
                ViewModel.Chart.Sensor = sensor;
                this.grdChart.RowDefinitions[1].Height = new GridLength(300);
                grid_SizeChanged(null, null);
            } else {
                this.grdChart.RowDefinitions[1].Height = new GridLength(0);
            }
        }

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
                } else if (item is BaseCondition) {
                    mnu = (ContextMenu)this.Resources["mnuCondition"];
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
        private void mnuProject_AddCondition_Click(object sender, EventArgs e)
        {
            var s = SelectedSensor;
            if (s != null) {

                // Pop up a window to design a new sensor
                var dlg = new AddObjectWindow(typeof(BaseCondition));
                var result = dlg.ShowDialog();
                if (result.HasValue && result.Value) {

                    // Add this sensor to the currently selected device
                    SensorProject.Current.AddCondition(s, dlg.ObjectToAdd as ICondition);
                }
            }
        }

        private void mnuProject_AddAction_Click(object sender, EventArgs e)
        {
            var c = SelectedCondition;
            if (c != null) {

                // Pop up a window to design a new sensor
                var dlg = new AddObjectWindow(typeof(BaseAction));
                var result = dlg.ShowDialog();
                if (result.HasValue && result.Value) {

                    // Add this sensor to the currently selected device
                    SensorProject.Current.AddAction(c, dlg.ObjectToAdd as IAction);
                }
            }
        }

        private void mnuProject_AddSensor_Click(object sender, EventArgs e)
        {
            var d = SelectedDevice;
            if (d != null) {

                // Pop up a window to design a new sensor
                var dlg = new AddObjectWindow(typeof(BaseSensor));
                var result = dlg.ShowDialog();
                if (result.HasValue && result.Value) {

                    // Add this sensor to the currently selected device
                    SensorProject.Current.AddSensor(d, dlg.ObjectToAdd as ISensor);
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

        private void mnuProject_ConditionTest_Click(object sender, RoutedEventArgs e)
        {
        }

        private void mnuProject_RemoveSensor_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSensor != null) {
                SelectedSensor.Parent.RemoveChild(SelectedSensor);
            }
        }

        private void mnuProject_RemoveCondition_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCondition != null) {
                SelectedCondition.Parent.RemoveChild(SelectedCondition);
            }
        }

        private void mnuProject_RemoveAction_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAction != null) {
                SelectedAction.Parent.RemoveChild(SelectedAction);
            }
        }
        #endregion

        #region Charting
        private void grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var width = grdChart.ColumnDefinitions[0].ActualWidth;
            var height = grdChart.RowDefinitions[1].ActualHeight;
            ViewModel.Chart.SetSize((int)width, (int)height);
        }
        #endregion

        #region Closing
        private bool isClosing = false;
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            isClosing = true;
            SensorProject.Current.Stop();
        }
        #endregion
    }
}
