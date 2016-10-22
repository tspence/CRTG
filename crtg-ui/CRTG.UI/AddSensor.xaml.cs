using CRTG.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace CRTG.UI
{
    /// <summary>
    /// Interaction logic for AddSensor.xaml
    /// </summary>
    public partial class AddSensor : Window
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
                    SensorToAdd = null;
                } else {
                    SensorToAdd = Activator.CreateInstance(SelectedItem) as ISensor;
                }
                ctlDisplayObject.DisplayObject = SensorToAdd;
            }
        }
        private Type _selectedItem;

        public ISensor SensorToAdd { get; set; }

        public AddSensor()
        {
            // Identify all types
            SensorTypes = new ObservableCollection<Type>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type t in a.GetTypes()) {
                    if (t.IsSubclassOf(typeof(BaseSensor))) {
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
            if (SelectedItem != null && SensorToAdd != null) {
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
