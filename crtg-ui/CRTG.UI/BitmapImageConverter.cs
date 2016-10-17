using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CRTG.UI
{
    public class BitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var imageDetail = (ImageDetail)value;
            var configs = ServiceLocator.Current.GetInstance<IRuntimeConfigurations>();
            var path = Path.Combine(configs.rootImagePath, imageDetail.fileName);
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            return new BitmapImage(uri);
        }
    }
}
