using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.UI.Models
{
    public interface ITreeViewModel
    {
        string Name { get; set; }
        Image Image { get; set; }
        List<ITreeViewModel> Children { get; set; }
    }
}
