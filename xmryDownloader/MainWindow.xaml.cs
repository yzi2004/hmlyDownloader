using System;
using System.Collections.Generic;
using System.IO;
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

namespace xmryDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }


        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtAlbumId.Text))
            {
                MessageBox.Show("要先输入专辑ID哦！");
                return;
            }


            WebAccess wa = new WebAccess();
            var data = await wa.GetAlbumTrackList(txtAlbumId.Text);


        }
    }
}
