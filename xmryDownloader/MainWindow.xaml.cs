using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using WinForm = System.Windows.Forms;

namespace xmryDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private xmryDownloadService _service = new xmryDownloadService();

        private TrackViewModel _viewModel;


        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAlbumId.Text))
            {
                MessageBox.Show("要先输入专辑ID哦！");
                return;
            }
            _viewModel = await _service.GetAlbumTrackList(txtAlbumId.Text, !(chkDesc.IsChecked ?? false));

            if (!string.IsNullOrWhiteSpace(_viewModel.ErrMsg))
            {
                MessageBox.Show(_viewModel.ErrMsg);
                return
                    ;
            }

            txtTitle.Text = _viewModel.Title;
            txtCount.Text = _viewModel.TrackCount.ToString();
            dgTrackList.ItemsSource = _viewModel.Data;
            radMp3.IsChecked = true;
            chkFileNo.IsChecked = true;
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPath.Text))
            {
                var dlg = new WinForm.FolderBrowserDialog();

                if (dlg.ShowDialog() == WinForm.DialogResult.OK)
                {
                    txtPath.Text = dlg.SelectedPath;
                }
                else
                {
                    return;
                }
            }

            string folder = EnsureFolder(txtPath.Text);
            int idx = 1;

            _viewModel.Data.ForEach(async item =>
            {

                string url = (radMp3.IsChecked ?? false) ? item.Mp3Url : item.M4aUrl;

                string destFile = $"{folder}\\{((chkFileNo.IsChecked ?? false) ? (idx++).ToString() + "." : "")}{Regex.Replace(item.Title, "[\\\\/<>\":*|?]", "")}.{((radMp3.IsChecked ?? false) ? "mp3" : "m4a")}";

                await _service.Download(url, destFile, (totalSize, totalRead, IsCompleted) =>
                {
                    if (totalSize > 0)
                    {
                        pbDownload.Value = totalRead * 100.0 / totalSize;
                    }
                });
            });
        }

        private void btnPath_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new WinForm.FolderBrowserDialog();

            if (dlg.ShowDialog() == WinForm.DialogResult.OK)
            {
                txtPath.Text = dlg.SelectedPath;
            }
        }

        private void radM4a_Checked(object sender, RoutedEventArgs e)
        {
            if (radM4a.IsChecked ?? false)
            {
                dgTrackList.Columns[2].Visibility = Visibility.Visible;
                dgTrackList.Columns[3].Visibility = Visibility.Hidden;
            }
        }

        private void radMp3_Checked(object sender, RoutedEventArgs e)
        {
            if (radMp3.IsChecked ?? false)
            {
                dgTrackList.Columns[2].Visibility = Visibility.Hidden;
                dgTrackList.Columns[3].Visibility = Visibility.Visible;
            }
        }

        private string EnsureFolder(string basePath)
        {
            string titleFolder = Regex.Replace(txtTitle.Text, "[\\\\/<>\":*|?]", "");
            string path = $"{basePath}\\{titleFolder}";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }
}
