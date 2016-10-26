using CRTG.Charts;
using CRTG.Common.Interfaces;
using CRTG.Sensors;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CRTG.UI
{
    public class SensorViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ISensorTreeModel> SensorTree { get; set; }

        public CrtgChart Chart { get; set; }

        #region Constructor
        public SensorViewModel()
        {
            // Make sure sensor project is loaded
            if (String.IsNullOrWhiteSpace(SensorProject.Current.Name)) {
                SensorProject.Current.Name = "CRTG Project";
            }

            // Set up the sensor tree
            SensorTree = new ObservableCollection<ISensorTreeModel>();
            SensorTree.Add(SensorProject.Current);
            Chart = new CrtgChart();
            Chart.DataStore = SensorProject.Current.DataStore;
            Chart.PropertyChanged += Chart_PropertyChanged;
        }

        private void Chart_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // The image changed!
            if (e.PropertyName == "ChartImage") {
                Notify("ChartImage");
            }
        }
        #endregion

        #region Notification
        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string property)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion
    }
}
