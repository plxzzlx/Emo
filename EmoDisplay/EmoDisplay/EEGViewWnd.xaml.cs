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
using System.Windows.Shapes;

namespace EmoDisplay
{
    /// <summary>
    /// EEGViewWnd.xaml 的交互逻辑
    /// </summary>
    public partial class EEGViewWnd : Window
    {
        DataManager.DataManager dm = null;
        public EEGViewWnd()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dm = DataManager.DataManager.GetInstance();
            dm.ConnectDevice("Emotiv");
            dm.RegistObserver(SrcView);
            SrcView.SetDevice(dm.GetDevice());
            SrcView.Start();
            dm.Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            dm.RemoveObserver(SrcView);
            SrcView.Stop();
        }
    }
}
