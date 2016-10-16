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

        private PropertyInfo[] _pilist;
        private List<AutoUIElement> _elements;
        protected Dictionary<string, StackPanel> _groups;

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
            _groups = new Dictionary<string, StackPanel>();
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
            // Construct the element for this item
            Label l = new Label();
            l.Content = e.ElementName();
            l.Margin = new Thickness(3);

            // Attach this to the appropriate stack panel
            var sp = _groups[e.GroupName()];
            sp.Children.Add(l);
        }

        private void AddGroup(string name)
        {
            // Make the internal stackpanel
            StackPanel sp = new StackPanel();
            sp.Margin = new Thickness(3);
            sp.Orientation = Orientation.Vertical;
            _groups[name] = sp;

            // Make the groupbox, and assign the stackpanel to its content
            GroupBox g = new GroupBox();
            g.Header = name;
            g.Margin = new Thickness(3);
            g.Content = sp;
            spRoot.Children.Add(g);
        }
        #endregion
    }
}