/*
 * 2012 - 2015 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/CRTG
 * 
 * This program uses icons from http://www.famfamfam.com/lab/icons/silk/
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Reflection;
using CRTG.UI.Helpers;
using CRTG.Sensors.Devices;
using CRTG.Sensors.SensorLibrary;
using CRTG.Sensors.Data;
using CRTG.Charts;
using CRTG.Notification;

namespace CRTG.UI
{
    public partial class frmCRTG : Form
    {
        public string _filename = Path.Combine(Application.StartupPath, "sensors.crtg");
        protected FormMapper _properties_map = null;

        #region Detecting visual selection state in the treeview
        public DeviceContext SelectedDevice
        {
            get
            {
                TreeNode tn = tvProject.SelectedNode;
                while (tn != null) {
                    if (tn != null && tn.Tag is DeviceContext) {
                        return (DeviceContext)tn.Tag;
                    }
                    tn = tn.Parent;
                }
                return null;
            }
        }
        public BaseSensor SelectedSensor
        {
            get
            {
                TreeNode tn = tvProject.SelectedNode;
                if (tn != null && tn.Tag is BaseSensor) {
                    return (BaseSensor)tn.Tag;
                }
                return null;
            }
            set
            {
                TreeNode[] list = tvProject.Nodes.Find("Sensor:" + value.Identity, true);
                if (list != null && list.Length == 1) {
                    tvProject.SelectedNode = list[0];
                }
            }
        }
        #endregion

        #region Populating information from the CRTG project
        public void UpdateTreeIcons()
        {
            foreach (DeviceContext dc in SensorProject.Current.Devices) {
                foreach (BaseSensor bs in dc.Sensors) {
                    TreeNode[] nodes = tvProject.Nodes.Find("Sensor:" + bs.Identity, true);
                    if (nodes != null && nodes.Length == 1) {
                        int newimage = ImageForSensor(bs);
                        if (nodes[0].ImageIndex != newimage) {
                            nodes[0].ImageIndex = newimage;
                            nodes[0].SelectedImageIndex = newimage;
                        }
                    }
                }
            }
        }

        public void LoadProjectTree()
        {
            string SelectedSensorId = tvProject.SelectedNode == null ? "" : tvProject.SelectedNode.Name;
            tvProject.BeginUpdate();
            tvProject.Nodes.Clear();

            // Iterate through all devices
            TreeNode project = new TreeNode();
            project.Text = "Sensor Project";
            project.ImageIndex = 3;
            project.SelectedImageIndex = 3;
            foreach (DeviceContext dc in SensorProject.Current.Devices) {
                project.Nodes.Add(MakeNode(dc));
            }
            tvProject.Nodes.Add(project);
            project.ExpandAll();

            // Find the node to focus
            TreeNode[] list = tvProject.Nodes.Find(SelectedSensorId, true);
            if (list.Length == 1) {
                tvProject.SelectedNode = list[0];
            }

            // Display the results
            tvProject.EndUpdate();
        }

        private TreeNode MakeNode(DeviceContext dc)
        {
            TreeNode tn = new TreeNode();
            tn.Text = dc.DeviceName;
            tn.Tag = dc;
            tn.Name = "Device:" + dc.Identity;
            tn.ImageIndex = 4;
            tn.SelectedImageIndex = 4;
            foreach (BaseSensor bs in dc.Sensors) {
                tn.Nodes.Add(MakeNode(bs));
            }
            return tn;
        }

        private TreeNode MakeNode(BaseSensor bs)
        {
            TreeNode tn = new TreeNode();
            tn.Text = bs.Name;
            tn.Tag = bs;
            tn.ImageIndex = ImageForSensor(bs);
            tn.SelectedImageIndex = tn.ImageIndex;
            tn.Name = "Sensor:" + bs.Identity;
            return tn;
        }

        private static int ImageForSensor(BaseSensor bs)
        {
            if (!bs.Enabled) {
                return 16;
            } else {
                switch (bs.CurrentState) {
                    case NotificationState.ErrorHigh:
                    case NotificationState.ErrorLow:
                        return 10;

                    case NotificationState.WarningHigh:
                    case NotificationState.WarningLow:
                        return 11;
                }
            }
            return 0;
        }
        #endregion

        #region Initialize the form
        public frmCRTG()
        {
            InitializeComponent();
            if (File.Exists(_filename)) {
                SensorProject.Current = SensorProject.Deserialize(_filename);
            } else {
                SensorProject.Current = new SensorProject();
            }
            SensorProject.Current.Notifications = new NotificationHelper();
        }

        private void frmCRTG_Load(object sender, EventArgs e)
        {
            // Load icons
            ilIcons.Images.Clear();
            ilIcons.Images.Add(Resource1.accept);
            ilIcons.Images.Add(Resource1.asterisk_orange);
            ilIcons.Images.Add(Resource1.cancel);
            ilIcons.Images.Add(Resource1.cog);
            ilIcons.Images.Add(Resource1.computer); 
            ilIcons.Images.Add(Resource1.flag_blue); //5
            ilIcons.Images.Add(Resource1.flag_green);
            ilIcons.Images.Add(Resource1.flag_orange);
            ilIcons.Images.Add(Resource1.flag_pink);
            ilIcons.Images.Add(Resource1.flag_purple);
            ilIcons.Images.Add(Resource1.flag_red); //10
            ilIcons.Images.Add(Resource1.flag_yellow);
            ilIcons.Images.Add(Resource1.folder);
            ilIcons.Images.Add(Resource1.hourglass);
            ilIcons.Images.Add(Resource1.lightning);
            ilIcons.Images.Add(Resource1.stop); //15
            ilIcons.Images.Add(Resource1.control_pause_blue);
            ilIcons.Images.Add(Resource1.email);
            tvProject.ImageList = ilIcons;

            // Set up the properties mapper
            _properties_map = new FormMapper(tabProperties);
            _properties_map.DataSaved += new EventHandler(_map_DataSaved);

            // Show data
            SensorProject.Current.Start();
            Rebind(true, true);
            ddlChartTime.SelectedIndex = 0;

            // Add menu items for all the available sensor types
            foreach (Type t in typeof(BaseSensor).Assembly.GetTypes()) {
                if (t.IsSubclassOf(typeof(BaseSensor))) {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(t.Name);
                    tsmi.Click += new EventHandler(this.sensorToolStripMenuItem_Click);
                    newDeviceToolStripMenuItem.DropDownItems.Add(tsmi);
                }
            }

            // Load time ranges
            this.ddlChartTime.Items.Clear();
            foreach (var tr in Enum.GetValues(typeof(ViewTimeframe))) {
                this.ddlChartTime.Items.Add(tr);
            }
        }

        void _map_DataSaved(object sender, EventArgs e)
        {
            Rebind(true, true);
            SaveSensors();
        }

        private void frmCRTG_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Closing CRTG will halt data collection.\r\n\r\nDo you wish to close CRTG?", "Confirm Close", MessageBoxButtons.YesNoCancel) != System.Windows.Forms.DialogResult.Yes) {
                e.Cancel = true;
            }
        }
        #endregion

        #region Bind data to the form
        protected DataTable _current_grid_contents = new DataTable();
        protected static DateTime _last_update_time = DateTime.MinValue;

        private void Rebind(bool force = false, bool update_tree = true)
        {
            System.Diagnostics.Debug.WriteLine("Rebind ({0} force, {1} forcetree)", force, update_tree);
            try {

                // Redo the project tree
                if (update_tree) {
                    LoadProjectTree();
                } else {
                    UpdateTreeIcons();
                }

                // Sensors have this behavior
                if (SelectedSensor != null) {
                    ShowTabs(true, true);

                    // Check to see if the properties tab needs updating
                    if (tabSensor.SelectedTab == tabMeasurements) {
                        RebindMeasurements(force);
                    } else if (tabSensor.SelectedTab == tabProperties) {
                        RebindProperties(force);
                    } else if (tabSensor.SelectedTab == tabErrors) {
                        RebindErrors(force);
                    }

                // If you've selected a device, we have this behavior
                } else {
                    ShowTabs(false, false);
                    RebindProperties(force);
                }

            // Catch and log an exception
            } catch (Exception ex) {
                SensorProject.LogException("Rebind", ex);
            }
        }

        private void ShowTabs(bool show_measurements, bool show_errors)
        {
            ShowTab(tabMeasurements, show_measurements);
            ShowTab(tabErrors, show_errors);
            if (tabSensor.SelectedTab == null) {
                tabSensor.SelectedTab = tabSensor.TabPages[0];
            }
        }

        private void ShowTab(TabPage tab, bool show)
        {
            bool is_shown = tabSensor.TabPages.Contains(tab);
            if (show && !is_shown) {
                tabSensor.TabPages.Add(tab);
            }
            if (!show && is_shown) {
                tabSensor.TabPages.Remove(tab);
            }
        }

        private void RebindErrors(bool force)
        {
            if (SelectedSensor == null) {
                lblCollectStatus.Text = "";
                txtLastCollect.Text = "";
                txtNextCollect.Text = "";
                txtCurrentError.Text = "";
                btnCheckNow.Enabled = false;
                btnClearException.Enabled = false;
            } else {
                if (SelectedSensor.InFlight) {
                    lblCollectStatus.Text = "Sensor is currently collecting data...";
                } else if (SelectedSensor.InError) {
                    lblCollectStatus.Text = "Sensor encountered an exception and is paused.";
                } else if (!SelectedSensor.Enabled) {
                    lblCollectStatus.Text = "Sensor is paused.";
                } else {
                    lblCollectStatus.Text = "Sensor is in " + SelectedSensor.CurrentState.ToString();
                }

                // Only show last collection if there was any
                if (SelectedSensor.SensorDataFile != null && SelectedSensor.SensorDataFile.Count > 0) {
                    txtLastCollect.Text = SelectedSensor.LastCollectTime.ToString();
                } else {
                    txtLastCollect.Text = "Never";
                }
                txtNextCollect.Text = SelectedSensor.NextCollectTime.ToString();
                txtCurrentError.Text = SelectedSensor.LastException;
                btnCheckNow.Enabled = SelectedSensor.Enabled && !SelectedSensor.InError && !SelectedSensor.InFlight;
                btnClearException.Enabled = SelectedSensor.InError;
            }
        }

        private object _current_properties_binding = null;

        private void RebindProperties(bool force)
        {
            object new_properties_binding = null;

            // Figure out what we should bind
            if (SelectedSensor != null) {
                new_properties_binding = SelectedSensor;
            } else if (SelectedDevice != null) {
                new_properties_binding = SelectedDevice;
            } else if (SensorProject.Current != null) {
                new_properties_binding = SensorProject.Current;
            } else {
                tabProperties.Controls.Clear();
            }

            // If this is a change, bind and populate it!
            if (new_properties_binding != _current_properties_binding) {
                _current_properties_binding = new_properties_binding;
                if (_current_properties_binding != null) {
                    _properties_map.Populate(_current_properties_binding.GetType(), _current_properties_binding, true);
                } else {
                    tabProperties.Controls.Clear();
                }
            }
        }

        private static DateTime _last_rebind = DateTime.MinValue;
        private static BaseSensor _last_rebind_sensor = null;

        private void RebindSensor()
        {
            // If we do not have a sensor selected, exit early
            if (SelectedSensor == null) return;

            // If the sensor hasn't been updated since last refresh, exit early
            if (_last_rebind_sensor == SelectedSensor && SelectedSensor.LastCollectTime < _last_rebind) return;
            _last_rebind_sensor = SelectedSensor;
            _last_rebind = DateTime.UtcNow;

            // Pass this off to the selected tab
            if (tabSensor.SelectedTab == tabMeasurements) {
                RebindMeasurements(false);
            } else if (tabSensor.SelectedTab == tabErrors) {
                RebindErrors(false);
            }
        }

        private CrtgChart _current_measurements = null;

        private void RebindMeasurements(bool force)
        {
            if (SelectedSensor == null) return;

            // Gather data information
            int data_size = 0;
            if (SelectedSensor != null && SelectedSensor.SensorDataFile != null) {
                data_size = SelectedSensor.SensorDataFile.Count;
            }
            bool sensor_updated = false;
            if ((SelectedSensor != null) && (SelectedSensor.LastCollectTime > _last_update_time)) {
                sensor_updated = true;
            }

            // Only change things if the number of rows we want to show is different from our current row count
            if ((force) || (_current_grid_contents == null) || (sensor_updated)) {
                if (SelectedSensor != null) {
                    _last_update_time = SelectedSensor.LastCollectTime;
                }

                // Dispose of previous measurements
                if (_current_measurements != null) {
                    _current_measurements.Dispose();
                    _current_measurements = null;
                }

                // Retrieve current information for display
                ViewTimeframe vt =  ViewTimeframe.Day;
                if (ddlChartTime.SelectedItem != null) {
                    vt = (ViewTimeframe)ddlChartTime.SelectedItem;
                }
                _current_measurements = ChartHelper.GetDisplayPackage(SelectedSensor, vt, pbChart.Width, pbChart.Height);

                // Show measurements
                pbChart.Image = _current_measurements.ChartImage;
                grdSensorData.SuspendLayout();
                grdSensorData.DataSource = _current_measurements.RawData;
                grdSensorData.ResumeLayout();
            }
        }
        #endregion

        #region Update the current values every 5 seconds
        protected bool _in_edit = false;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!_in_edit) {
                RebindSensor();
            } else {
                UpdateTreeIcons();
            }
        }

        private void grdSensors_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            _in_edit = false;
            SaveSensors();
        }

        private void grdSensors_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            _in_edit = true;
        }
        #endregion

        #region Sensor context menu
        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (SelectedSensor != null) {
                if (MessageBox.Show("Do you wish to remove this sensor?", "Confirm Remove Sensor", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes) {
                    SelectedSensor.Device.Sensors.Remove(SelectedSensor);
                    SaveSensors();
                    Rebind(true, true);
                }
            }
        }

        private void emptyHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedSensor != null) {
                if (MessageBox.Show("Do you wish to delete all data for this sensor?", "Confirm Empty History", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes) {
                    SelectedSensor.ClearAllData();
                }
            }
        }

        private void enableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedSensor != null) {
                SelectedSensor.Enabled = !SelectedSensor.Enabled;
                LoadProjectTree();
            }
        }

        private void checkNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedSensor != null) {
                SelectedSensor.NextCollectTime = DateTime.UtcNow;
            }
        }

        private void resetErrorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InnerClearException(SelectedSensor);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlSensor ss = new SqlSensor();
            SensorProject.Current.AddSensor(SelectedDevice, ss);
            SelectedSensor = ss;
            SaveSensors();
            Rebind(true, true);
        }
        #endregion

        #region Saving the sensor project
        private void SaveSensors()
        {
            if (File.Exists(_filename)) {
                File.Delete(_filename);
            }
            SensorProject.Current.Serialize(_filename);
        }

        private void frmCRTG_FormClosed(object sender, FormClosedEventArgs e)
        {
            SensorProject.Current.Stop();
            SaveSensors();
        }
        #endregion

        #region Treeview selections
        private TreeNode _current_hilite_node = null;
        private void tvProject_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _last_update_time = DateTime.MinValue;
            if (_current_hilite_node != null) {
                _current_hilite_node.BackColor = Color.FromKnownColor(KnownColor.Window);
                _current_hilite_node.ForeColor = Color.FromKnownColor(KnownColor.WindowText);
            }
            _current_hilite_node = tvProject.SelectedNode;
            _current_hilite_node.BackColor = Color.FromKnownColor(KnownColor.Highlight);
            _current_hilite_node.ForeColor = Color.FromKnownColor(KnownColor.HighlightText);
            Rebind(true, false);

            // Make context menu items relevant
            resetErrorToolStripMenuItem.Enabled = (SelectedSensor != null && SelectedSensor.InError);
            checkNowToolStripMenuItem.Enabled = (SelectedSensor != null);
            enableToolStripMenuItem.Enabled = (SelectedSensor != null);
        }

        private void tvProject_MouseDown(object sender, MouseEventArgs e)
        {
            // User right clicked, select the node they clicked
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                var node = tvProject.HitTest(e.X, e.Y).Node;
                tvProject.SelectedNode = node;
            }

            // Update the context menu before showing it
            if (SelectedSensor != null) {
                enableToolStripMenuItem.Text = (SelectedSensor.Enabled ? "&Pause" : "&Resume");
                resetErrorToolStripMenuItem.Enabled = SelectedSensor.InError;
            }

            // If this is a right click, show the correct context menu
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {

                // Show the correct context menu
                if (SelectedSensor == null) {
                    mnuDevices.Show(tvProject, e.Location);
                } else {
                    mnuSensor.Show(tvProject, e.Location);
                }
            }
        }
        #endregion

        #region Chart events
        private void pbChart_SizeChanged(object sender, EventArgs e)
        {
            Rebind(true, false);
        }

        private void ddlChartTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            RebindMeasurements(true);
        }
        #endregion

        #region Top menu
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout dlg = new frmAbout();
            dlg.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void uTCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SensorProject.Current.TimeZonePreference = DateTimePreference.UTC;
            //uTCToolStripMenuItem.Checked = true;
            //localTimeToolStripMenuItem.Checked = false;
        }

        private void localTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SensorProject.Current.TimeZonePreference = DateTimePreference.LocalTime;
            //uTCToolStripMenuItem.Checked = false;
            //localTimeToolStripMenuItem.Checked = true;
        }
        #endregion

        #region Context menus
        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedSensor != null) {
                var dupe = SelectedSensor.Duplicate();
                SensorProject.Current.AddSensor(SelectedSensor.Device, dupe);
                SaveSensors();
                Rebind(true, true);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedDevice == null) return;
            if (MessageBox.Show(String.Format("Do you really wish to delete '{0}'?", SelectedDevice.DeviceName), "Confirm Delete", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes) {
                SensorProject.Current.Devices.Remove(SelectedDevice);
                SaveSensors();
                Rebind(true, true);
            }
        }

        private void deviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeviceContext dc = new DeviceContext();
            dc.DeviceName = "New Device";
            SensorProject.Current.AddDevice(dc);
            SaveSensors();
            Rebind(true, true);
        }

        private void sensorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Figure out the typename of the sensor
            Assembly a = typeof(BaseSensor).Assembly;
            string name = ((ToolStripMenuItem)sender).Text;
            foreach (Type t in a.GetTypes()) {
                if (t.Name == name) {

                    // Make an instance of this type
                    BaseSensor bs = (BaseSensor)a.CreateInstance(t.FullName, true);
                    bs.Name = "New " + bs.GetType().Name;
                    bs.Frequency = Interval.FifteenMinutes;
                    SensorProject.Current.AddSensor(SelectedDevice, bs);
                    SaveSensors();
                    Rebind(true, true);
                    return;
                }
            }
        }
        #endregion

        #region Tab controls
        private void btnCheckNow_Click(object sender, EventArgs e)
        {
            if (SelectedSensor != null) {
                SelectedSensor.NextCollectTime = DateTime.UtcNow;
                Rebind(true, true);
            }
        }

        private void btnClearException_Click(object sender, EventArgs e)
        {
            InnerClearException(SelectedSensor);
        }

        private void InnerClearException(BaseSensor bs) 
        {
            if (bs != null) {
                bs.InError = false;
                bs.ErrorMessage = "";
                bs.LastException = "";
                RebindErrors(true);
            }
        }

        private void tabSensor_TabIndexChanged(object sender, EventArgs e)
        {
            Rebind(true, false);
        }

        private void tabSensor_SelectedIndexChanged(object sender, EventArgs e)
        {
            Rebind(true, false);
        }
        #endregion

        #region Show tooltip when hovering over the chart so you can observe values for a particular time range
        private void pbChart_MouseMove(object sender, MouseEventArgs e)
        {

        }
        #endregion
    }
}
