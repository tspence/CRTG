using CRTG.Common;
using CRTG.Common.Data;
using CRTG.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Actions
{
    public class BaseCondition : BaseSensorTreeModel, ICondition
    {
        public List<IAction> Actions { get; set; }
        public string Condition { get; set; }
        public IDataStore DataStore { get; set; }
        
        #region Link to sensor
        /// <summary>
        /// Sensor this condition is attached to
        /// </summary>
        public ISensor Sensor
        {
            get
            {
                return _sensor;
            }
            set
            {
                if (_sensor != null) {
                    _sensor.SensorCollect -= Sensor_PropertyChanged;
                }
                _sensor = value;
                if (_sensor != null) {
                    _sensor.SensorCollect += Sensor_PropertyChanged;
                }
                Notify("Sensor");
            }
        }

        /// <summary>
        /// Capture new data and test it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sensor_PropertyChanged(ISensor sender, SensorCollectEventArgs e)
        {
            if (Test(e)) {
                foreach (var action in Actions) {
                    action.Execute(DataStore, Sensor, this, e);
                }
            }
        }
        #endregion

        private ISensor _sensor;

        /// <summary>
        /// Execute this condition and return true if it meets the threshold
        /// </summary>
        /// <returns></returns>
        public virtual bool Test(SensorCollectEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
