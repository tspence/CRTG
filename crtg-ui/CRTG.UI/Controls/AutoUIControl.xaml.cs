using CRTG.Common.Attributes;
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
    /// Interaction logic for AutoUIControl.xaml
    /// </summary>
    public partial class AutoUIControl : UserControl, INotifyPropertyChanged
    {
        private class AutoUIElement
        {
            public string GroupName()
            {
                return ui.Group ?? "";
            }

            public string ElementName()
            {
                return ui.Label ?? property.Name;
            }

            public AutoUI ui { get; set; }
            public PropertyInfo property { get; set; }
        }

        private class GroupBoxElement
        {
            public Grid grid { get; set; }
            public int row { get; set; }

            public void AddControl(Control ctl, int col)
            {
                ctl.Margin = new Thickness(3);
                ctl.SetValue(Grid.RowProperty, row);
                ctl.SetValue(Grid.ColumnProperty, col);
                grid.Children.Add(ctl);
            }

            public void NextRow()
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                row++;
            }
        }

        private PropertyInfo[] _pilist;
        private List<AutoUIElement> _elements;
        private Dictionary<string, GroupBoxElement> _groups;

        /// <summary>
        /// Controls the object to display
        /// </summary>
        public Object DisplayObject {
            get
            {
                return _obj;
            }

            set
            {
                _obj = value;
                GenerateUI();
                Notify("DisplayObject");
            }
        }
        private Object _obj;


        public AutoUIControl()
        {
            InitializeComponent();
        }

        #region Notification
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string name)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        #region UI Generator
        public void GenerateUI()
        {
            spRoot.Children.Clear();

            // Assemble a list of all items to show in this UI
            var t = DisplayObject.GetType();
            _pilist = t.GetProperties();
            _elements = new List<AutoUIElement>();
            _groups = new Dictionary<string, GroupBoxElement>();
            foreach (var pi in _pilist) {
                var ui = pi.GetCustomAttribute<AutoUI>();
                if (ui != null && !ui.Skip) _elements.Add(new AutoUIElement() { ui = ui, property = pi });
            }

            // Now assemble an alphabetical list of group boxes
            var groupnames = (from e in _elements orderby e.GroupName() select e.GroupName()).Distinct();
            foreach (var name in groupnames) {
                AddGroup(name);
            }

            // Now attach all controls to group boxes
            foreach (var e in _elements) {
                AddElement(e);
            }
        }

        private void AddElement(AutoUIElement e)
        {
            // Figure out what stack panel to attach this element to, and create a horizontal panel
            //var sp = new StackPanel();
            //sp.Width = double.NaN;
            //sp.Orientation = Orientation.Horizontal;
            var gbe = _groups[e.GroupName()]; //.Children.Add(sp);

            // Is this nullable?
            bool is_nullable = false;
            var vartype = e.property.PropertyType;
            if (vartype.IsGenericType && vartype.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                vartype = Nullable.GetUnderlyingType(vartype);
                is_nullable = true;
            }

            // Make the label
            Label lbl = new Label();
            lbl.Content = e.ElementName();
            lbl.HorizontalAlignment = HorizontalAlignment.Right;
            gbe.AddControl(lbl, 0);

            // If this item is nullable, insert a checkbox here
            if (is_nullable) {
                var chk = new CheckBox();
                gbe.AddControl(chk, 1);
            }

            // Determine tooltip
            string tooltip = e.ui.Help ?? vartype.ToString();
            Control ctl = null;

            // Enum variable controls
            if (vartype.IsEnum) {
                var ddl = new ComboBox();
                ddl.Items.Add("(select)");
                foreach (var v in Enum.GetValues(vartype)) {
                    ddl.Items.Add(v.ToString());
                }
                ddl.SelectedIndex = 0;
                ddl.ToolTip = tooltip;
                ctl = ddl;

            // DateTime objects use the date pickers
            } else if (vartype == typeof(DateTime)) {
                DatePicker dtp = new DatePicker();
                dtp.ToolTip = tooltip;
                ctl = dtp;

            // Booleans are just checkboxes, or dropdowns
            } else if (vartype == typeof(bool)) {
                ComboBox ddl = new ComboBox();
                ddl.Items.Add("False");
                ddl.Items.Add("True");
                ddl.SelectedIndex = 0;
                ddl.ToolTip = tooltip;
                ctl = ddl;

            // Multi line text boxes go hereText boxes use this control
            } else if (e.ui.MultiLine > 1) {
                TextBox tb = new TextBox();
                tb.AcceptsReturn = true;
                tb.Height = 150;
                tb.FontFamily = new FontFamily("Courier New");
                tb.ToolTip = tooltip;
                ScrollViewer sv = new ScrollViewer();
                sv.Content = tb;
                ctl = sv;

            // Password fields use PasswordBox
            } else if (e.ui.PasswordField) {
                PasswordBox pb = new PasswordBox();
                pb.ToolTip = tooltip;
                ctl = pb;

            // File browsers go here
            } else if (e.ui.BrowseFile) {
                TextBox tb = new TextBox();
                tb.ToolTip = tooltip;
                ctl = tb;

                // Browse button
                Button b = new Button();
                b.Width = 30;
                b.Content = "...";
                b.Click += BrowseFileClick;
                b.Tag = tb;
                gbe.AddControl(b, 3);

                // Folder browsers go here
            } else if (e.ui.BrowseFolder) {
                TextBox tb = new TextBox();
                tb.ToolTip = tooltip;
                ctl = tb;

                // Browse button
                Button b = new Button();
                b.Width = 30;
                b.Content = "...";
                b.Tag = tb;
                b.Click += BrowseFolderClick;
                gbe.AddControl(b, 3);

            // Regular text boxes go here
            } else {
                TextBox tb = new TextBox();
                tb.ToolTip = tooltip;
                ctl = tb;
            }

            // Is the parameter optional?
            if (e.ui.ReadOnly) {
                ctl.IsEnabled = false;
            }

            // Make control stretch
            gbe.AddControl(ctl, 2);
            gbe.NextRow();
        }

        private void BrowseFolderClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Browse Folder");
        }

        private void BrowseFileClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Browse File");
        }

        private void AddGroup(string name)
        {
            // Make the internal stackpanel
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1.0, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            // Make the groupbox, and assign the stackpanel to its content
            GroupBox g = new GroupBox();
            g.Header = name;
            g.Margin = new Thickness(3);
            g.Content = grid;
            spRoot.Children.Add(g);

            // Remember what we did
            GroupBoxElement gbe = new GroupBoxElement();
            gbe.grid = grid;
            gbe.row = 0;
            _groups[name] = gbe;
        }
        #endregion
    }
}