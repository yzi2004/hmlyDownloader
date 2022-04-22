using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private bool _mp3Flg = true;

        public bool UseMp3Fmt
        {
            get => _mp3Flg;
            set => SetProperty(ref _mp3Flg, value);
        }

        public bool UseM4aFmt
        {
            get => !_mp3Flg;
            set => SetProperty(ref _mp3Flg, !value);
        }

        private bool _addFileNo;

        public bool AddFileNo
        {
            get => _addFileNo;
            set => SetProperty(ref _addFileNo, value);
        }

        private string _albumTitle;
        public string AlbumTitle
        {
            get => _albumTitle;
            set => SetProperty(ref _albumTitle, value);
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

        private double _downloadProgress;

        public double DownloadProgress
        {
            get => _downloadProgress;
            set => SetProperty(ref _downloadProgress, value);
        }

        private double _totalProgess;
        public double TotalProgress
        {
            get => _totalProgess;
            set => SetProperty(ref _totalProgess, value);
        }


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
                var trackList = await _service.GetAlbumTrackList(AlbumID, UseMp3Fmt, !IsDesc);

                Data = new ObservableCollection<TrackItem>(trackList);

                OnPropertyChanged(nameof(Data));
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
                AlbumTitle = Data[0].AlbumTitle;
                TrackCount = Data.Count;
            }
        });

        public ICommand DoDownload => new AsyncRelayCommand(async () =>
        {
            for (int idx = 0; idx < Data.Count; idx++)
            {
                var item = Data[idx];

                //string destFile = $"{Utils.EnsureFolder(Path, AlbumTitle)}\\{((AddFileNo) ? idx.ToString() + "." : "")}{Utils.EnsureFileName(item.TrackTitle)}.{(UseMp3Fmt ? "mp3" : "m4a")}";

                ////await _service.Download(item.DownloadUrl, destFile, (totalSize, totalRead, IsCompleted) =>
                ////{
                ////    if (totalSize > 0)
                ////    {
                ////        DownloadProgress = totalRead * 100.0 / totalSize;
                ////    }
                ////});

                //TotalProgress = (idx + 1) * 100.0 / Data.Count;
                item.DownloadUrl += "[OK]"; 
                //OnPropertyChanged(nameof(Data));
                Thread.Sleep(1000);
            }
        });

        public ICommand SelPath => new RelayCommand(() =>
        {
            var dlg = new DialogModel() { action = DialogModel.ACTION.selfolder };

            WeakReferenceMessenger.Default.Send(dlg);

            Path = dlg.result;

        });

        public ObservableCollection<TrackItem> Data { get; set; }
    }

    internal class TrackItem
    {
        public string AlbumTitle { get; set; }
        public int No { get; set; }

        public int Id { get; set; }

        public string TrackTitle { get; set; }

        public TimeSpan Duration { get; set; }

        public string DownloadUrl { get; set; }

        public bool Downloaded { get; set; }
    }
}
