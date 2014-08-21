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
        DataManager.DataWriter dw = null;
        bool IsSaving = false;
        public EEGViewWnd()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
             
            
            dm = DataManager.DataManager.GetInstance();
            dm.ConnectDevice("Emotiv");
            dm.RegistObserver(SrcView);
            dw = new DataManager.DataWriter("D:", "data.dat");
            //dm.RegistObserver(dw);

            SrcView.SetDevice(dm.GetDevice());
            SrcView.Start();
            dm.Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            dm.RemoveObserver(dw);
            dm.RemoveObserver(SrcView);
            SrcView.Stop();
        }

        private void Tag_Click(object sender, RoutedEventArgs e)
        {
            int simulus = Convert.ToInt32(SimulusCodeBox.Text);
            int source = Convert.ToInt32(SourceTimeBox.Text);
            dw.SimulusCode = simulus;
            dw.SourceTime = source;
        }

        private void SaveDataBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsSaving)
            {
                dm.RemoveObserver(dw);
                SaveDataBtn.Content = "保存数据";
                Tag.IsEnabled = false;
            }
            else
            {
                dm.RegistObserver(dw);
                SaveDataBtn.Content = "停止保存";
                Tag.IsEnabled = true;
            }
             IsSaving = !IsSaving;   
        }
    }
}
