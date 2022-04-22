using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using WinForm = System.Windows.Forms;

namespace hmlyDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<DialogModel>(this, Receive);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
            base.OnClosing(e);
        }

        public void Receive(object rec, DialogModel message)
        {
            if (message.action == DialogModel.ACTION.showmsg)
            {
                MessageBox.Show(message.message);
            }
            if (message.action == DialogModel.ACTION.selfolder)
            {
                var dlg = new WinForm.FolderBrowserDialog();

                if (dlg.ShowDialog() == WinForm.DialogResult.OK)
                {
                    message.result = dlg.SelectedPath;
                }
            }
        }
    }
}
