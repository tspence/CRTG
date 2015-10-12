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
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using CRTG.Sensors;

namespace CRTG.UI.Helpers
{
    #region Form Mapper for automatic UI generation
    public class FormMapper
    {
        /// <summary>
        /// Shared class for tooltips
        /// </summary>
        public ToolTip Tips = new ToolTip();

        /// <summary>
        /// Shared fonts for multiline text boxes
        /// </summary>
        public static System.Drawing.Font CourierFont = new System.Drawing.Font("Courier New", 8);

        /// <summary>
        /// This is the next top position, vertically descending, for the next control added to this autoform
        /// </summary>
        private int _next_top = 10;

        protected Type _type = null;
        protected Object _obj = null;
        protected Control _container;
        protected Dictionary<string, GroupBox> _groups = new Dictionary<string, GroupBox>();

        public FormMapper(Control container)
            : base()
        {
            _container = container;
        }

        #region Events
        public event EventHandler DataSaved;

        protected void OnDataSaved()
        {
            if (DataSaved != null) {
                DataSaved(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Public interface
        /// <summary>
        /// Save values from the form to the object
        /// </summary>
        /// <param name="o"></param>
        public void SetToObject()
        {
            object o;

            // Iterate through fields & properties
            PropertyInfo[] pilist = _type.GetProperties();
            for (int i = 0; i < pilist.Length; i++) {
                if (TryGetValueFromControl(pilist[i].PropertyType, "p" + i.ToString(), out o)) {
                    pilist[i].SetValue(_obj, o, null);
                }
            }
            FieldInfo[] filist = _type.GetFields();
            for (int i = 0; i < filist.Length; i++) {
                if (TryGetValueFromControl(filist[i].FieldType, "f" + i.ToString(), out o)) {
                    filist[i].SetValue(_obj, o);
                }
            }

            // Notify everyone
            OnDataSaved();
        }

        /// <summary>
        /// The currently selected object
        /// </summary>
        public Object CurrentObject
        {
            get
            {
                return _obj;
            }
        }

        /// <summary>
        /// Automatically generate the user interface for the selected type, and optionally an object
        /// </summary>
        /// <param name="t">The type to use for displaying user interface</param>
        /// <param name="o">The object to retrieve data from, if any</param>
        public void Populate(Type t, object o, bool WithSaveButton)
        {
            // If we're already pointed to this object, do no work
            if (o == _obj) return;

            // Clean up any prior work and keep track of who we are editing
            _obj = o;
            _type = t;
            if (_container.Controls.Count > 0) {
                _container.Controls[0].Focus();
            }
            _container.SuspendLayout();
            _container.Controls.Clear();
            _next_top = 10;
            _groups = new Dictionary<string, GroupBox>();

            // Add a blank control to the top left hand corner for scrolling purposes
            Control c = new Control();
            c.Top = 0;
            c.Left = 0;
            c.Height = 1;
            c.Width = 1;
            c.Parent = _container;

            // Iterate through fields
            PropertyInfo[] pilist = t.GetProperties();
            for (int i = 0; i < pilist.Length; i++) {

                // Assemble enough information to make a control for it
                PropertyInfo pi = pilist[i];
                object val = null;
                if (_obj != null) {
                    val = pilist[i].GetValue(_obj, null);
                }
                GenerateControlsForVariable("p" + i.ToString(), pi.Name, pi.PropertyType, pi.GetUI(), false, val);
            }

            // Now do the same for fields
            FieldInfo[] filist = t.GetFields();
            for (int i = 0; i < filist.Length; i++) {

                // Assemble enough information to make a control for it
                FieldInfo fi = filist[i];
                object val = null;
                if (_obj != null) {
                    val = filist[i].GetValue(_obj);
                }
                GenerateControlsForVariable("f" + i.ToString(), fi.Name, fi.FieldType, fi.GetUI(), false, val);
            }

            // Now fixup all the controls
            int top = 10;
            int max_width = 0;
            foreach (GroupBox gb in _groups.Values) {
                gb.Top = top;
                FixupControlPositionsAndHeight(gb);
                top = gb.Top + gb.Height + 10;
                if (gb.Left + gb.Width > max_width) {
                    max_width = gb.Left + gb.Width;
                }
            }

            // Add save button now
            if (WithSaveButton) {
                Button save = new Button();
                save.Text = "Save";
                save.Top = top;
                save.Left = max_width - save.Width;
                save.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                save.Click += new EventHandler(save_Click);
                _container.Controls.Add(save);
            }
            _container.ResumeLayout();
        }
        #endregion

        #region Inner helpers
        /// <summary>
        /// Retrieve a value from a control
        /// </summary>
        /// <param name="value_type"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool TryGetValueFromControl(Type value_type, string name, out object value)
        {
            value = null;

            // Is this an optional parameter or a string that could be null?  Skip it if so
            Control c = _container.Controls.Find("check" + name, true).FirstOrDefault();
            if (c is CheckBox) {
                if (!((CheckBox)c).Checked) {
                    return true;
                }
            }

            // Find our control!
            c = _container.Controls.Find("ctl" + name, true).FirstOrDefault();
            if (c == null) return false;

            // Can we parse its value?
            object val = null;
            if (c is ComboBox) {
                val = ((ComboBox)c).SelectedItem;
            } else if (c is DateTimePicker) {
                val = ((DateTimePicker)c).Value;
            } else if (c is TextBox) {
                val = ((TextBox)c).Text.Replace("\r", "");
            }

            // Convert the parameter to something that can be used - if we fail, beep and highlight
            try {
                if (value_type.IsEnum) {
                    value = Enum.Parse(value_type, val.ToString());
                    return true;
                } else {
                    if (value_type.IsGenericType && value_type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                        value = Convert.ChangeType(val, Nullable.GetUnderlyingType(value_type));
                    } else {
                        value = Convert.ChangeType(val, value_type);
                    }
                    return true;
                }
            } catch {
                Console.WriteLine("Unable to parse");
            }
            return false;
        }

        /// <summary>
        /// Trigger saving to the object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void save_Click(object sender, EventArgs e)
        {
            SetToObject();
        }

        /// <summary>
        /// Changes the control's size to full width, anchor top | left | right, and returns the next "top" position vertically
        /// </summary>
        /// <param name="c"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        protected int AddFullWidthControl(Control c, int top)
        {
            c.Left = 10;
            c.Top = top;
            c.Width = _container.Width - 35;
            c.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _container.Controls.Add(c);
            _next_top = c.Top + c.Height + 10;
            return _next_top;
        }

        /// <summary>
        /// Shortcut for adding a control
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected int AddFullWidthControl(Control c)
        {
            return AddFullWidthControl(c, _next_top);
        }

        /// <summary>
        /// Fixup all controls within a group box, using labels on the left and controls on the right, with checkboxes in the middle
        /// </summary>
        /// <param name="g"></param>
        protected void FixupControlPositionsAndHeight(GroupBox g)
        {
            int MaxWidth = 0;
            int MaxHeight = 0;
            int check_adj = 0;

            // Find maximum label width and keep track of text widths of labels
            foreach (Control c in g.Controls) {
                if (c is Label) {
                    int text_size = TextRenderer.MeasureText(((Label)c).Text, ((Label)c).Font).Width;
                    MaxWidth = Math.Max(MaxWidth, text_size);
                    c.Width = text_size;
                }
                if (c.Name.StartsWith("check")) {
                    check_adj = 25;
                }
                MaxHeight = Math.Max(MaxHeight, c.Top + c.Height);
            }

            // Move everything right 10
            MaxWidth += 10;

            // Reset control positions by width
            foreach (Control c in g.Controls) {
                if (c.Name.StartsWith("lbl")) {
                    c.Left = MaxWidth - c.Width;
                } else if (c.Name.StartsWith("check")) {
                    c.Left = MaxWidth + 10;
                } else if (c.Name.StartsWith("btn")) {
                } else {
                    c.Left = MaxWidth + 10 + check_adj;
                    c.Width = g.Width - MaxWidth - 20 - check_adj;
                }
            }

            // Reset height of this group box
            g.Height = MaxHeight + 10;
        }

        /// <summary>
        /// Generate a new groupbox and add it to this contorl
        /// </summary>
        /// <param name="f"></param>
        /// <param name="previous"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected GroupBox NextGroupBox(string name)
        {
            GroupBox gb = new GroupBox();
            gb.Text = name;
            gb.Height = 20;
            AddFullWidthControl(gb);
            return gb;
        }

        /// <summary>
        /// Generate all the necessary UI for this data entry
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="name"></param>
        /// <param name="ui"></param>
        /// <param name="read_only"></param>
        /// <param name="default_value"></param>
        private void GenerateControlsForVariable(string identifier, string name, Type vartype, AutoUI ui, bool read_only, object default_value)
        {
            if (ui != null && ui.Skip) return;

            // Produce the correct values, whether this parameter has an AutoUI or not
            string group = "Common";
            string help = null;
            string label = name;
            int lines = 1;
            GroupBox gb = null;
            bool browse_file = false;
            bool browse_folder = false;
            if (ui != null) {
                browse_file = ui.BrowseFile;
                browse_folder = ui.BrowseFolder;
                if (!String.IsNullOrEmpty(ui.Group)) group = ui.Group;
                help = ui.Help;
                if (!String.IsNullOrEmpty(ui.Label)) label = ui.Label;
                lines = ui.MultiLine;
            }

            // Find the relevant group box
            if (!_groups.TryGetValue(group, out gb)) {
                gb = NextGroupBox(group);
                _groups[group] = gb;
            }

            // Is this nullable?
            bool is_nullable = false;
            if (vartype.IsGenericType && vartype.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                vartype = Nullable.GetUnderlyingType(vartype);
                is_nullable = true;
            }

            // Make the label
            Label lbl = new Label();
            lbl.Name = "lbl" + identifier;
            lbl.Text = name;
            lbl.Left = 10;
            lbl.Width = 100;
            lbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            lbl.Anchor = AnchorStyles.Left | AnchorStyles.Top;

            // Make a control for the data entry.  Extra is for controls on the right hand side of the screen, if any.
            Control ctl = null;
            Control extra = null;

            // Enum variable controls
            if (vartype.IsEnum) {
                ComboBox ddl = new ComboBox();
                ddl.DropDownStyle = ComboBoxStyle.DropDownList;
                ddl.Items.Add("(select)");
                foreach (var v in Enum.GetValues(vartype)) {
                    ddl.Items.Add(v.ToString());
                }
                ddl.SelectedIndex = 0;
                if (default_value != null) {
                    string s = default_value.ToString();
                    for (int i = 0; i < ddl.Items.Count; i++) {
                        if (string.Equals(s, (string)ddl.Items[i])) {
                            ddl.SelectedIndex = i;
                            ddl.Tag = ddl.SelectedIndex;
                        }
                    }
                }
                ctl = ddl;
            } else if (vartype == typeof(DateTime)) {
                DateTimePicker dtp = new DateTimePicker();
                if (default_value is DateTime) {
                    dtp.Value = (DateTime)default_value;
                    dtp.Tag = dtp.Value;
                }
                ctl = dtp;
            } else if (vartype == typeof(bool)) {
                ComboBox ddl = new ComboBox();
                ddl.DropDownStyle = ComboBoxStyle.DropDownList;
                ddl.Items.Add("False");
                ddl.Items.Add("True");
                ddl.SelectedIndex = 0;
                if (default_value is bool) {
                    if ((bool)default_value) {
                        ddl.SelectedIndex = 1;
                        ddl.Tag = ddl.SelectedIndex;
                    }
                }
                ctl = ddl;
            } else {
                TypedTextBox tb = new TypedTextBox(vartype);
                if (lines > 1) {
                    tb.Multiline = true;
                    tb.Font = CourierFont;
                    tb.ScrollBars = ScrollBars.Vertical;
                    tb.Height = tb.Height * lines;
                    if (ui.PasswordField) {
                        tb.PasswordChar = '*';
                    }
                }

                // Convert \n to \r\n
                if (default_value != null) {
                    tb.Text = default_value.ToString().Replace("\n", "\r\n");
                    tb.Tag = tb.Text;
                }
                ctl = tb;

                // Did we need to add an extra browse button?
                if (browse_file) {
                    Button b = new Button();
                    b.Name = "btnBrowseFile" + identifier;
                    b.Text = "...";
                    b.Tag = ctl;
                    b.Click += new EventHandler(button_BrowseToFile);
                    b.Width = 30;
                    extra = b;
                } else if (browse_folder) {
                    Button b = new Button();
                    b.Name = "btnBrowseFolder" + identifier;
                    b.Text = "...";
                    b.Tag = ctl;
                    b.Click += new EventHandler(button_BrowseToFolder);
                    b.Width = 30;
                    extra = b;
                }
            }
            ctl.Width = gb.Width - 100 - 30;
            ctl.Left = 100 + 20;
            ctl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            ctl.Name = "ctl" + identifier;

            // Set the tooltip if one exists
            if (!String.IsNullOrEmpty(help)) {
                Tips.SetToolTip(ctl, String.Format("{0} ({1})", help, vartype.ToString()));
            } else {
                Tips.SetToolTip(ctl, vartype.ToString());
            }

            // Is the parameter optional?
            CheckBox chk = null;
            if (read_only) {
                ctl.Enabled = false;
            } else {
                if (is_nullable) {
                    chk = new CheckBox();
                    chk.Name = "check" + identifier;
                    chk.Left = ctl.Left;
                    chk.Checked = (default_value != null);
                    chk.CheckedChanged += new EventHandler(check_CheckedChanged);
                    chk.Width = chk.Height;
                    ctl.Left = chk.Left + chk.Width + 10;
                    ctl.Width = ctl.Width - chk.Width - 10;
                }
            }

            // Add to the parent group box
            int top = gb.Height;
            lbl.Top = top;
            ctl.Top = top;
            gb.Controls.Add(lbl);

            // Is there a checkbox at the left?
            if (chk != null) {
                chk.Top = top;
                gb.Controls.Add(chk);
                ctl.Enabled = chk.Checked;
            }

            // Is there an extra control on the right?
            if (extra != null) {
                extra.Top = top;
                extra.Left = gb.Width - 10 - extra.Width;
                extra.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                ctl.Width = ctl.Width - 20 - extra.Width;
                gb.Controls.Add(extra);
            }
            gb.Controls.Add(ctl);
            gb.Height = ctl.Top + ctl.Height + 10;
        }

        void button_BrowseToFile(object sender, EventArgs e)
        {
            // Retrieve current information
            Control c = sender as Control;
            TypedTextBox tb = c.Tag as TypedTextBox;

            // Launch this in a dialog
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files (*.*)|*.*||";
            dlg.Title = "Select a file...";
            dlg.CheckFileExists = false;
            dlg.CheckPathExists = false;
            dlg.ValidateNames = false;
            dlg.FileName = tb.Text;
            if (dlg.ShowDialog() == DialogResult.OK) {
                tb.Text = dlg.FileName;
            }
        }

        void button_BrowseToFolder(object sender, EventArgs e)
        {
            // Retrieve information from existing controls
            Control c = sender as Control;
            TypedTextBox tb = c.Tag as TypedTextBox;

            // Show a dialog
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Select a folder...";
            dlg.SelectedPath = tb.Text;
            if (dlg.ShowDialog() == DialogResult.OK) {
                tb.Text = dlg.SelectedPath;
            }
        }

        /// <summary>
        /// Enable or disable the corresponding data control when the user toggles a parameter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void check_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            // Find the matching data control
            Control c = cb.Parent.Parent.Controls.Find("ctl" + cb.Name.Substring(5), true).FirstOrDefault();
            if (c != null) {
                c.Enabled = cb.Checked;
                if (!c.Enabled) {
                    c.ResetText();
                }
            }
        }
        #endregion
    }
    #endregion

}
