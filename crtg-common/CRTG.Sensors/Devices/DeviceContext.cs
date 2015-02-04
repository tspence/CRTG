/*
 * 2006 - 2012 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://code.google.com/p/csharp-striker/
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace CRTG.Sensors.Devices
{
    [Serializable]
    public class DeviceContext
    {
        [AutoUI(Skip=true)]
        public List<BaseSensor> Sensors = new List<BaseSensor>();

        /// <summary>
        /// Serialized ID number of the device
        /// </summary>
        [AutoUI(Skip=true)]
        public int Identity;

        /// <summary>
        /// Human readable name describing the device
        /// </summary>
        [AutoUI(Group = "Device")]
        public string DeviceName;

        [AutoUI(Group = "Device", Help="DNS name or IP address.")]
        public string Address;

        [AutoUI(Group = "Device")]
        public DeviceType Category;

        [AutoUI(Label="Domain", Group = "Windows Credentials")]
        public string WinDomain;

        [AutoUI(Label="Username", Group = "Windows Credentials")]
        public string WinUsername;

        [AutoUI(Label="Password", Group = "Windows Credentials")]
        public string WinPassword;

        [AutoUI(Group = "Notification", MultiLine=10, Help = "This information can be provided in emails to help provide context.")]
        public string DeviceInformation;


        #region Helper Functions
        /// <summary>
        /// Query the device using WMI
        /// </summary>
        /// <returns></returns>
        public ManagementObjectCollection WmiQuery(string querytext)
        {
            ManagementScope scope = null;

            // If we have an account
            if (!String.IsNullOrEmpty(WinUsername)) {
                ConnectionOptions connection = new ConnectionOptions();
                connection.Username = WinUsername;
                connection.Password = WinPassword;
                connection.Authority = "ntlmdomain:" + WinDomain;
                scope = new ManagementScope("\\\\" + Address + "\\root\\CIMV2", connection);
                scope.Connect();

            // Assume we can connect to the computer without an account
            } else {
                scope = new ManagementScope("\\\\" + Address + "\\root\\CIMV2");
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
