using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.Common.Interfaces
{
    public interface ISensorTreeModel
    {
        string Name { get; }
        string IconPath { get; }
        IEnumerable<ISensorTreeModel> Children { get; }
    }
}
