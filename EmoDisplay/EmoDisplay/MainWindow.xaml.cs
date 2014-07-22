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
using DataManager;

namespace EmoDisplay
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DataManager.DataManager dm = null;
        public MainWindow()
        {
            InitializeComponent();
            dm = DataManager.DataManager.GetInstance();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            LoadDeviceWnd ldw = new LoadDeviceWnd();
            ldw.Show();
        }

        private void EEG_Click(object sender, RoutedEventArgs e)
        {
            EEGViewWnd eegview = new EEGViewWnd();
            eegview.Show();
        }

        private void FFT_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Brain_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            dm.Stop();
        }
    }
}
