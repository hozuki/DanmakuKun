using System;
using System.Windows;

namespace DanmakuKun
{
    public static class WindowExtension
    {

        public static void ShowDialog(this Window window, Window owner)
        {
            window.Owner = owner;
            window.ShowDialog();
        }

        public static void ShowDialogEx(this Window window)
        {
            window.ShowDialog(Application.Current.MainWindow);
        }

    }
}
