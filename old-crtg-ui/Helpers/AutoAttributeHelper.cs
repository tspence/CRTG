using CRTG.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.UI.Helpers
{
    #region Attribute helpers for the UI
    public static class AutoAttributeHelper
    {
        /// <summary>
        /// Returns the UI information for this property, if available
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static AutoUI GetUI(this PropertyInfo pi)
        {
            object[] olist = pi.GetCustomAttributes(typeof(AutoUI), false);
            if (olist != null && olist.Length == 1) {
                return olist[0] as AutoUI;
            }
            return null;
        }

        /// <summary>
        /// Returns the UI information for this field, if available
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        public static AutoUI GetUI(this FieldInfo fi)
        {
            object[] olist = fi.GetCustomAttributes(typeof(AutoUI), false);
            if (olist != null && olist.Length == 1) {
                return olist[0] as AutoUI;
            }
            return null;
        }
    }
    #endregion

}
