using CRTG.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Sensors.Toolkit
{
    public static class WmiHelper
    {
        #region Helper Functions
        /// <summary>
        /// Query the device using WMI
        /// </summary>
        /// <returns></returns>
        public static ManagementObjectCollection WmiQuery(IDevice device, string querytext)
        {
            ManagementScope scope = null;

            // Determine correct address, or assume local computer if none
            string scope_path = "\\root\\CIMV2";
            if (!String.IsNullOrEmpty(device.Address)) {

                // For remote connections, we have to use credentials
                scope_path = "\\\\" + device.Address + scope_path;
                ConnectionOptions connection = new ConnectionOptions();
                connection.Username = device.Username;
                connection.Password = device.Password;
                connection.Authority = "ntlmdomain:" + device.WindowsDomain;
                scope = new ManagementScope(scope_path, connection);
                scope.Connect();

                // Assume we can connect to the computer without an account
            } else {
                scope = new ManagementScope(scope_path);
            }

            // Okay, connect and run the query
            scope.Connect();
            ObjectQuery query = new ObjectQuery(querytext);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            return searcher.Get();
        }
        #endregion
    }
}
