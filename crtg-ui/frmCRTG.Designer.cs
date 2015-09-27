namespace CRTG.UI
{
    partial class frmCRTG
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.mnuSensor = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.checkNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.resetErrorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.emptyHistoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvProject = new System.Windows.Forms.TreeView();
            this.tabSensor = new System.Windows.Forms.TabControl();
            this.tabMeasurements = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.pbChart = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ddlChartTime = new System.Windows.Forms.ComboBox();
            this.grdSensorData = new System.Windows.Forms.DataGridView();
            this.tabProperties = new System.Windows.Forms.TabPage();
            this.tabErrors = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblCollectStatus = new System.Windows.Forms.Label();
            this.btnClearException = new System.Windows.Forms.Button();
            this.txtCurrentError = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCheckNow = new System.Windows.Forms.Button();
            this.txtNextCollect = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLastCollect = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
            this.mnuDevices = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.statusStrip1.SuspendLayout();
            this.mnuSensor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabSensor.SuspendLayout();
            this.tabMeasurements.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdSensorData)).BeginInit();
            this.tabErrors.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.mnuDevices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 541);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(846, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(250, 17);
            this.toolStripStatusLabel1.Text = "Right click on the sensor list to make changes.";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // mnuSensor
            // 
            this.mnuSensor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkNowToolStripMenuItem,
            this.enableToolStripMenuItem,
            this.toolStripSeparator3,
            this.resetErrorToolStripMenuItem,
            this.emptyHistoryToolStripMenuItem,
            this.toolStripSeparator4,
            this.deleteToolStripMenuItem1,
            this.duplicateToolStripMenuItem});
            this.mnuSensor.Name = "mnuSensor";
            this.mnuSensor.Size = new System.Drawing.Size(150, 148);
            // 
            // checkNowToolStripMenuItem
            // 
            this.checkNowToolStripMenuItem.Name = "checkNowToolStripMenuItem";
            this.checkNowToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.checkNowToolStripMenuItem.Text = "&Check Now";
            this.checkNowToolStripMenuItem.Click += new System.EventHandler(this.checkNowToolStripMenuItem_Click);
            // 
            // enableToolStripMenuItem
            // 
            this.enableToolStripMenuItem.Name = "enableToolStripMenuItem";
            this.enableToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.enableToolStripMenuItem.Text = "&Pause";
            this.enableToolStripMenuItem.Click += new System.EventHandler(this.enableToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(146, 6);
            // 
            // resetErrorToolStripMenuItem
            // 
            this.resetErrorToolStripMenuItem.Name = "resetErrorToolStripMenuItem";
            this.resetErrorToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.resetErrorToolStripMenuItem.Text = "&Reset Error";
            this.resetErrorToolStripMenuItem.Click += new System.EventHandler(this.resetErrorToolStripMenuItem_Click);
            // 
            // emptyHistoryToolStripMenuItem
            // 
            this.emptyHistoryToolStripMenuItem.Name = "emptyHistoryToolStripMenuItem";
            this.emptyHistoryToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.emptyHistoryToolStripMenuItem.Text = "Empty &History";
            this.emptyHistoryToolStripMenuItem.Click += new System.EventHandler(this.emptyHistoryToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(146, 6);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(149, 22);
            this.deleteToolStripMenuItem1.Text = "&Delete...";
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
            // 
            // duplicateToolStripMenuItem
            // 
            this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
            this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.duplicateToolStripMenuItem.Text = "Du&plicate...";
            this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvProject);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabSensor);
            this.splitContainer1.Size = new System.Drawing.Size(846, 517);
            this.splitContainer1.SplitterDistance = 282;
            this.splitContainer1.TabIndex = 5;
            // 
            // tvProject
            // 
            this.tvProject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvProject.Location = new System.Drawing.Point(0, 0);
            this.tvProject.Name = "tvProject";
            this.tvProject.Size = new System.Drawing.Size(282, 517);
            this.tvProject.TabIndex = 0;
            this.tvProject.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvProject_AfterSelect);
            this.tvProject.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvProject_MouseDown);
            // 
            // tabSensor
            // 
            this.tabSensor.Controls.Add(this.tabMeasurements);
            this.tabSensor.Controls.Add(this.tabProperties);
            this.tabSensor.Controls.Add(this.tabErrors);
            this.tabSensor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSensor.Location = new System.Drawing.Point(0, 0);
            this.tabSensor.Name = "tabSensor";
            this.tabSensor.SelectedIndex = 0;
            this.tabSensor.Size = new System.Drawing.Size(560, 517);
            this.tabSensor.TabIndex = 0;
            this.tabSensor.SelectedIndexChanged += new System.EventHandler(this.tabSensor_SelectedIndexChanged);
            this.tabSensor.TabIndexChanged += new System.EventHandler(this.tabSensor_TabIndexChanged);
            // 
            // tabMeasurements
            // 
            this.tabMeasurements.Controls.Add(this.splitContainer2);
            this.tabMeasurements.Location = new System.Drawing.Point(4, 22);
            this.tabMeasurements.Name = "tabMeasurements";
            this.tabMeasurements.Padding = new System.Windows.Forms.Padding(3);
            this.tabMeasurements.Size = new System.Drawing.Size(552, 491);
            this.tabMeasurements.TabIndex = 0;
            this.tabMeasurements.Text = "Measurements";
            this.tabMeasurements.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.pbChart);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.ddlChartTime);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.grdSensorData);
            this.splitContainer2.Size = new System.Drawing.Size(546, 485);
            this.splitContainer2.SplitterDistance = 166;
            this.splitContainer2.TabIndex = 1;
            // 
            // pbChart
            // 
            this.pbChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbChart.Location = new System.Drawing.Point(6, 30);
            this.pbChart.Name = "pbChart";
            this.pbChart.Size = new System.Drawing.Size(537, 133);
            this.pbChart.TabIndex = 3;
            this.pbChart.TabStop = false;
            this.pbChart.SizeChanged += new System.EventHandler(this.pbChart_SizeChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Time Range";
            // 
            // ddlChartTime
            // 
            this.ddlChartTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ddlChartTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlChartTime.FormattingEnabled = true;
            this.ddlChartTime.Items.AddRange(new object[] {
            "15 Minutes",
            "1 Hour",
            "24 Hours",
            "7 Days",
            "30 Days",
            "All Time"});
            this.ddlChartTime.Location = new System.Drawing.Point(74, 3);
            this.ddlChartTime.Name = "ddlChartTime";
            this.ddlChartTime.Size = new System.Drawing.Size(469, 21);
            this.ddlChartTime.TabIndex = 1;
            this.ddlChartTime.SelectedIndexChanged += new System.EventHandler(this.ddlChartTime_SelectedIndexChanged);
            // 
            // grdSensorData
            // 
            this.grdSensorData.AllowUserToAddRows = false;
            this.grdSensorData.AllowUserToDeleteRows = false;
            this.grdSensorData.AllowUserToResizeRows = false;
            this.grdSensorData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdSensorData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdSensorData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdSensorData.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.grdSensorData.Location = new System.Drawing.Point(0, 0);
            this.grdSensorData.Name = "grdSensorData";
            this.grdSensorData.ReadOnly = true;
            this.grdSensorData.RowHeadersVisible = false;
            this.grdSensorData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdSensorData.Size = new System.Drawing.Size(546, 315);
            this.grdSensorData.TabIndex = 0;
            // 
            // tabProperties
            // 
            this.tabProperties.AutoScroll = true;
            this.tabProperties.Location = new System.Drawing.Point(4, 22);
            this.tabProperties.Name = "tabProperties";
            this.tabProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tabProperties.Size = new System.Drawing.Size(552, 491);
            this.tabProperties.TabIndex = 1;
            this.tabProperties.Text = "Properties";
            this.tabProperties.UseVisualStyleBackColor = true;
            // 
            // tabErrors
            // 
            this.tabErrors.Controls.Add(this.groupBox2);
            this.tabErrors.Controls.Add(this.groupBox1);
            this.tabErrors.Location = new System.Drawing.Point(4, 22);
            this.tabErrors.Name = "tabErrors";
            this.tabErrors.Padding = new System.Windows.Forms.Padding(3);
            this.tabErrors.Size = new System.Drawing.Size(552, 491);
            this.tabErrors.TabIndex = 2;
            this.tabErrors.Text = "Status";
            this.tabErrors.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lblCollectStatus);
            this.groupBox2.Controls.Add(this.btnClearException);
            this.groupBox2.Controls.Add(this.txtCurrentError);
            this.groupBox2.Location = new System.Drawing.Point(6, 112);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(538, 373);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Error Conditions";
            // 
            // lblCollectStatus
            // 
            this.lblCollectStatus.AutoSize = true;
            this.lblCollectStatus.Location = new System.Drawing.Point(6, 29);
            this.lblCollectStatus.Name = "lblCollectStatus";
            this.lblCollectStatus.Size = new System.Drawing.Size(35, 13);
            this.lblCollectStatus.TabIndex = 2;
            this.lblCollectStatus.Text = "label4";
            // 
            // btnClearException
            // 
            this.btnClearException.Location = new System.Drawing.Point(457, 19);
            this.btnClearException.Name = "btnClearException";
            this.btnClearException.Size = new System.Drawing.Size(75, 23);
            this.btnClearException.TabIndex = 1;
            this.btnClearException.Text = "&Clear";
            this.btnClearException.UseVisualStyleBackColor = true;
            this.btnClearException.Click += new System.EventHandler(this.btnClearException_Click);
            // 
            // txtCurrentError
            // 
            this.txtCurrentError.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCurrentError.Location = new System.Drawing.Point(6, 48);
            this.txtCurrentError.Multiline = true;
            this.txtCurrentError.Name = "txtCurrentError";
            this.txtCurrentError.ReadOnly = true;
            this.txtCurrentError.Size = new System.Drawing.Size(526, 319);
            this.txtCurrentError.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnCheckNow);
            this.groupBox1.Controls.Add(this.txtNextCollect);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtLastCollect);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(538, 100);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sensor Status";
            // 
            // btnCheckNow
            // 
            this.btnCheckNow.Location = new System.Drawing.Point(286, 50);
            this.btnCheckNow.Name = "btnCheckNow";
            this.btnCheckNow.Size = new System.Drawing.Size(83, 23);
            this.btnCheckNow.TabIndex = 2;
            this.btnCheckNow.Text = "Check &Now";
            this.btnCheckNow.UseVisualStyleBackColor = true;
            this.btnCheckNow.Click += new System.EventHandler(this.btnCheckNow_Click);
            // 
            // txtNextCollect
            // 
            this.txtNextCollect.Location = new System.Drawing.Point(83, 52);
            this.txtNextCollect.Name = "txtNextCollect";
            this.txtNextCollect.ReadOnly = true;
            this.txtNextCollect.Size = new System.Drawing.Size(197, 20);
            this.txtNextCollect.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Next Collect";
            // 
            // txtLastCollect
            // 
            this.txtLastCollect.Location = new System.Drawing.Point(83, 26);
            this.txtLastCollect.Name = "txtLastCollect";
            this.txtLastCollect.ReadOnly = true;
            this.txtLastCollect.Size = new System.Drawing.Size(197, 20);
            this.txtLastCollect.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Last Collect";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(846, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // ilIcons
            // 
            this.ilIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            this.ilIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // mnuDevices
            // 
            this.mnuDevices.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newDeviceToolStripMenuItem,
            this.toolStripSeparator2,
            this.deleteToolStripMenuItem});
            this.mnuDevices.Name = "mnuDevices";
            this.mnuDevices.Size = new System.Drawing.Size(117, 54);
            // 
            // newDeviceToolStripMenuItem
            // 
            this.newDeviceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deviceToolStripMenuItem,
            this.toolStripSeparator1});
            this.newDeviceToolStripMenuItem.Name = "newDeviceToolStripMenuItem";
            this.newDeviceToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.newDeviceToolStripMenuItem.Text = "&New";
            // 
            // deviceToolStripMenuItem
            // 
            this.deviceToolStripMenuItem.Name = "deviceToolStripMenuItem";
            this.deviceToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.deviceToolStripMenuItem.Text = "&Device";
            this.deviceToolStripMenuItem.Click += new System.EventHandler(this.deviceToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(106, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(113, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.deleteToolStripMenuItem.Text = "&Delete...";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataSource = typeof(CRTG.BaseSensor);
            // 
            // frmCRTG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(846, 563);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmCRTG";
            this.Text = "CRTG";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCRTG_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmCRTG_FormClosed);
            this.Load += new System.EventHandler(this.frmCRTG_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.mnuSensor.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabSensor.ResumeLayout(false);
            this.tabMeasurements.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdSensorData)).EndInit();
            this.tabErrors.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.mnuDevices.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip mnuSensor;
        private System.Windows.Forms.ToolStripMenuItem enableToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem resetErrorToolStripMenuItem;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.ToolStripMenuItem checkNowToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView tvProject;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ImageList ilIcons;
        private System.Windows.Forms.ContextMenuStrip mnuDevices;
        private System.Windows.Forms.ToolStripMenuItem newDeviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.TabControl tabSensor;
        private System.Windows.Forms.TabPage tabMeasurements;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ddlChartTime;
        private System.Windows.Forms.DataGridView grdSensorData;
        private System.Windows.Forms.TabPage tabProperties;
        private System.Windows.Forms.ToolStripMenuItem deviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem emptyHistoryToolStripMenuItem;
        private System.Windows.Forms.TabPage tabErrors;
        private System.Windows.Forms.TextBox txtCurrentError;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnClearException;
        private System.Windows.Forms.TextBox txtNextCollect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLastCollect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCheckNow;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.Label lblCollectStatus;
        private System.Windows.Forms.ToolStripMenuItem duplicateToolStripMenuItem;
        private System.Windows.Forms.PictureBox pbChart;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}

