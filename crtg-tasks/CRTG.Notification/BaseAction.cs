using CRTG.Common;
using CRTG.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRTG.Common.Data;

namespace CRTG.Actions
{
    public class BaseAction : BaseSensorTreeModel, IAction
    {
        public string Description { get; set; }
        public ISensor Sensor { get; set; }

        #region Action implementation
        /// <summary>
        /// Override this in child classes to develop more actions
        /// </summary>
        /// <param name="datastore"></param>
        /// <param name="sensor"></param>
        /// <param name="condition"></param>
        /// <param name="args"></param>
        public virtual void Execute(IDataStore datastore, ISensor sensor, ICondition condition, SensorCollectEventArgs args)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
