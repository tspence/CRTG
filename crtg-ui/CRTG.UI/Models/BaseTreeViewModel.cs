using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRTG.UI.Models
{
    public class BaseTreeViewModel : ITreeViewModel, INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private Image _image;
        public Image Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                NotifyPropertyChanged("Image");
            }
        }

        private List<ITreeViewModel> _children;
        public List<ITreeViewModel> Children
        {
            get
            {
                return _children;
            }
            set
            {
                _children = value;
                NotifyPropertyChanged("Children");
            }
        }

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
