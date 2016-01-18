using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Common
{
    public interface IDevice
    {
        List<ISensor> Sensors { get; set; }
        int Identity { get; set; }
        string DeviceName { get; set; }
        string Address { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string WindowsDomain { get; set; }
        DeviceType Category { get; set; }
        string DeviceInformation { get; set; }
        string ODBCConnectionString { get; set; }
    }
}
