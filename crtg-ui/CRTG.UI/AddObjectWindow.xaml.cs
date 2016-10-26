using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;

namespace CRTG.UI
{
    /// <summary>
    /// Interaction logic for AddSensor.xaml
    /// </summary>
    public partial class AddObjectWindow : Window
    {
        public ObservableCollection<Type> SensorTypes { get; set; }

        public Type SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                if (_selectedItem == null) {
                    ObjectToAdd = null;
                } else {
                    ObjectToAdd = Activator.CreateInstance(SelectedItem);
                }
                ctlDisplayObject.DisplayObject = ObjectToAdd;
            }
        }
        private Type _selectedItem;

        public object ObjectToAdd { get; set; }

        public AddObjectWindow(Type baseType)
        {
            // Identify all types
            SensorTypes = new ObservableCollection<Type>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type t in a.GetTypes()) {
                    if (t.IsSubclassOf(baseType)) {
                        SensorTypes.Add(t);
                    }
                }
            }
            this.DataContext = this;

            // Show the UI
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null && ObjectToAdd != null) {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
