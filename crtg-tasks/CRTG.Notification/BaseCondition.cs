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
        public string Condition { get; set; }
        public IDataStore DataStore { get; set; }
        public ISensor Sensor { get; set; }

        /// <summary>
        /// Execute this condition and return true if it meets the threshold
        /// </summary>
        /// <returns></returns>
        public virtual void TestCondition(SensorCollectEventArgs args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Trigger all actions
        /// </summary>
        /// <param name="args"></param>
        public virtual void TriggerActions(SensorCollectEventArgs args)
        {
            foreach (IAction act in Children) {
                act.Execute(DataStore, Sensor, this, args);
            }
        }
    }
}
