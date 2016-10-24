using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Common.Data
{
    public class CollectResult
    {
        public Decimal Value { get; set; }
        public Dictionary<string, object> Properties { get; set; }

        #region Constructors
        public CollectResult(decimal value)
        {
            Value = value;
            Properties = new Dictionary<string, object>();
        }

        public CollectResult(decimal value, DataTable report)
        {
            Value = value;
            Properties = new Dictionary<string, object>();
            Properties["Report"] = report;
        }
        #endregion
    }
}
