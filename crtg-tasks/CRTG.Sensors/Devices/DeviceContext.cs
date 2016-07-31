/*
 * 2012 - 2016 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/CRTG
 * 
 * This program uses icons from http://www.famfamfam.com/lab/icons/silk/
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using CRTG.Common;

namespace CRTG.Sensors.Devices
{
    [Serializable]
    public class DeviceContext : IDevice
    {
        [AutoUI(Skip=true)]
        public List<ISensor> Sensors { get; set; }

        /// <summary>
        /// Serialized ID number of the device
        /// </summary>
        [AutoUI(Skip = true)]
        public int Identity { get; set; }

        /// <summary>
        /// Human readable name describing the device
        /// </summary>
        [AutoUI(Group = "Device")]
        public string DeviceName { get; set; }

        [AutoUI(Group = "Device", Help="DNS name or IP address.")]
        public string Address { get; set; }

        [AutoUI(Group = "Device")]
        public DeviceType Category { get; set; }

        [AutoUI(Label="Domain", Group = "Windows Credentials")]
        public string WindowsDomain { get; set; }

        [AutoUI(Label="Username", Group = "Windows Credentials")]
        public string Username { get; set; }

        [AutoUI(Label="Password", Group = "Windows Credentials", PasswordField=true)]
        public string Password { get; set; }

        [AutoUI(Group = "Notification", MultiLine=10, Help = "This information can be provided in emails to help provide context.")]
        public string DeviceInformation { get; set; }

        [AutoUI(Label = "ODBC Connection String", Group = "Database")]
        public string ConnectionString { get; set; }

        #region Constructor
        public DeviceContext()
        {
            Sensors = new List<ISensor>();
        }
        #endregion
    }
}
