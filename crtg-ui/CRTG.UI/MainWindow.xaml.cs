using CRTG.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CRTG.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<ISensorTreeModel> SensorTree { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // Let's set up some models!
            //SensorTree = new List<BaseTreeViewModel>();
            //SensorTree.Add(MakeSample(5));
            SensorTree = new List<ISensorTreeModel>();
            SensorTree.Add(SensorProject.Current);
            SensorProject.Current.Name = "Hi";
            tvSensors1.DataContext = this;
        }

        //private BaseTreeViewModel MakeSample(int v)
        //{
        //    BaseTreeViewModel m = new BaseTreeViewModel();
        //    m.Name = Guid.NewGuid().ToString();
        //    m.Children = new List<ITreeViewModel>();
        //    for (int i = 0; i < v; i++) {
        //        m.Children.Add(MakeSample(v - 1));
        //    }
        //    return m;
        //}
    }
}
