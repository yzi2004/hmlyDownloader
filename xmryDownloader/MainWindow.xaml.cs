using System.Windows;

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

            lblTitle.Content = _viewModel.Title;
            lblCount.Content = _viewModel.TrackCount.ToString();
            dgTrackList.ItemsSource = _viewModel.Data;
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            string url = "http://aod.cos.tx.xmcdn.com/group68/M0B/3C/72/wKgMeF3KmkqyCl04ACULolF_vV0371.m4a";

            _service.Download(url, "c:\\temp\\aaa.m4a");

        }
    }
}
