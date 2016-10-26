using CRTG.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Common.Interfaces
{
    public interface IAction : ISensorTreeModel
    {
        string Description { get; set; }

        /// <summary>
        /// Execute this action
        /// </summary>
        void Execute(IDataStore datastore, ISensor sensor, ICondition condition, SensorCollectEventArgs args);
    }
}
