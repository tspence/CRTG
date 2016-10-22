using CRTG.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Common.Interfaces
{
    public interface IDevice : ISensorTreeModel
    {
        int Identity { get; set; }
        string Address { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string WindowsDomain { get; set; }
        DeviceType Category { get; set; }
        string DeviceInformation { get; set; }
        string ConnectionString { get; set; }
        string DeviceName { get; set; }
    }
}
