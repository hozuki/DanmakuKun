using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DanmakuKun
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow : Window
    {

        private const string ACFUN_URL = @"http://www.acfun.tv/u/570460.aspx";
        private const string BILIBILI_URL = @"http://space.bilibili.com/2970191";

        public AboutWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NavigateToBiliBili_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(BILIBILI_URL);
        }

        private void NavigateToAcFun_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(ACFUN_URL);
        }

    }
}
