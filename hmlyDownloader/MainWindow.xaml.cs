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
            WeakReferenceMessenger.Default.Register<CustomMessenger>(this, Receive);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
            base.OnClosing(e);
        }

        public void Receive(object rec, CustomMessenger message)
        {
            if (message.action == CustomMessenger.ACTION.showmsg)
            {
                MessageBox.Show(message.message);
            }
            if (message.action == CustomMessenger.ACTION.selfolder)
            {
                var dlg = new WinForm.FolderBrowserDialog();

                if (dlg.ShowDialog() == WinForm.DialogResult.OK)
                {
                    message.result = dlg.SelectedPath;
                }
            }
            if (message.action == CustomMessenger.ACTION.setfocus)
            {
                if (message.message == "datagrid")
                {
                    this.dgTrackList.Focus();
                }
            }
            if (message.action == CustomMessenger.ACTION.scrollview)
            {
                this.dgTrackList.ScrollIntoView(dgTrackList.SelectedItem);
            }
        }
    }
}
