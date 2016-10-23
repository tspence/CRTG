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
        /// Executes the condition and returns true if the condition is met
        /// </summary>
        /// <returns></returns>
        bool Test(SensorCollectEventArgs args);
    }
}
