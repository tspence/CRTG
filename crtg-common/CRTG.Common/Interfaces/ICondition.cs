using CRTG.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Common.Interfaces
{
    public interface ICondition
    {
        List<IAction> Actions { get; set; }
        ISensor Sensor { get; set; }
        IDataStore DataStore { get; set; }

        /// <summary>
        /// Executes the condition and triggers actions if the condition is met
        /// </summary>
        /// <returns></returns>
        void TestCondition(SensorCollectEventArgs args);

        /// <summary>
        /// Triggers all actions and passes them the notification we received
        /// </summary>
        /// <param name="args"></param>
        void TriggerActions(SensorCollectEventArgs args);
    }
}
