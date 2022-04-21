using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Input;

namespace hmlyDownloader
{
    internal class MainWindowViewModel : ObservableRecipient
    {
        private IDownloadService? _service;

        public MainWindowViewModel()
        {
            _service = Ioc.Default.GetService<IDownloadService>();
        }

        private string _AlbumID;

        public string AlbumID
        {
            get => _AlbumID;
            set => SetProperty(ref _AlbumID, value);
        }

        private bool _descFlg;

        public bool IsDesc
        {
            get => _descFlg;
            set => SetProperty(ref _descFlg, value);
        }

        private bool _mp3Flg;

        public bool UseMp3Fmt
        {
            get => _mp3Flg;
            set => SetProperty(ref _mp3Flg, value);
        }

        private bool _m4aFlg;

        public bool UseM4aFmt
        {
            get => _m4aFlg;
            set => SetProperty(ref _m4aFlg, value);
        }

        private bool _addFileNo;

        public bool AddFileNo
        {
            get => _addFileNo;
            set => SetProperty(ref _addFileNo, value);
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private int? _trackCnt;

        public int? TrackCount
        {
            get => _trackCnt;
            set => SetProperty(ref _trackCnt, value);
        }

        private string _path;
        public string Path
        {
            get => _path;
            set => SetProperty(ref _path, value);
        }

        public string ErrMsg { get; set; }


        public ICommand DoLoad => new AsyncRelayCommand(async () =>
        {
            if (string.IsNullOrWhiteSpace(AlbumID))
            {
                WeakReferenceMessenger.Default.Send(new DialogModel()
                {
                    action = DialogModel.ACTION.showmsg,
                    message = "要先输入专辑ID哦！"
                });
                return;
            }
            try
            {
                Data = await _service.GetAlbumTrackList(AlbumID, !IsDesc);
            }
            catch (Exception ex)
            {
                WeakReferenceMessenger.Default.Send(new DialogModel()
                {
                    action = DialogModel.ACTION.showmsg,
                    message = ex.Message
                });
                return;
            }

            if (Data.Count > 0)
            {
                Title = Data[0].AlbumTitle;
                TrackCount = Data.Count;
                UseMp3Fmt = true;
                AddFileNo = true;
            }
        });

        public ICommand DoDownload => new RelayCommand(() =>
        {
            int idx = 0;
            Data.ForEach(async item =>
                 {
                     string url = UseMp3Fmt ? item.Mp3Url : item.M4aUrl;

                     string destFile = $"{_path}\\{((_addFileNo) ? idx.ToString() + "." : "")}{Regex.Replace(item.Title, "[\\\\/<>\":*|?]", "")}.{(_mp3Flg ? "mp3" : "m4a")}";

                     await _service.Download(url, destFile, (totalSize, totalRead, IsCompleted) =>
                     {
                         if (totalSize > 0)
                         {
                             //pbDownload.Value = totalRead * 100.0 / totalSize;
                             //pbTotal.Value = idx * 100.0 / _viewModel.Data.Count;
                         }
                     });
                     Thread.Sleep(1000);
                     idx++;
                 });
        });

        public ICommand SelPath => new RelayCommand(() =>
        {
            var dlg = new DialogModel() { action = DialogModel.ACTION.selfolder };

            WeakReferenceMessenger.Default.Send(dlg);

            Path = dlg.result;

            //Messenger.Send(new )


        });

        public List<TrackItem> Data { get; set; } = new List<TrackItem>();
    }

    internal class TrackItem
    {
        public string AlbumTitle { get; set; }
        public int No { get; set; }

        public int Id { get; set; }

        public string Title { get; set; }

        public TimeSpan Duration { get; set; }

        public string Mp3Url { get; set; }

        public string M4aUrl { get; set; }

    }
}
