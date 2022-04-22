using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace hmlyDownloader
{
    internal interface IDownloadService
    {
        public Task<List<TrackItem>> GetAlbumTrackList(string albumID, bool UseMp3Fmt = true, bool ascFlg = true);
        public Task Download(string url, string fileName, Action<long, long, bool> ReportProgess);
    }

    internal class DownloadService : IDownloadService
    {
        WebAccess wa = new WebAccess();

        public async Task<List<TrackItem>> GetAlbumTrackList(string albumID, bool UseMp3Fmt = true, bool ascFlg = true)
        {
            var data = new List<TrackItem>();
            var tracks = await GetAlbumTrackList(albumID, ascFlg, 1);

            if (tracks == null || tracks.ret != 0)
            {
                throw new Exception(string.IsNullOrWhiteSpace(tracks?.msg) ? "Noops,系统出错了。" : tracks.msg);
            }
            if ((tracks.data?.totalCount ?? 0) <= 0)
            {
                throw new Exception("Oh,Nooo, 为啥找不到音频文件呢。");
            }

            int No = 1;

            tracks.data.list.ForEach(item => data.Add(new TrackItem()
            {
                No = No++,
                AlbumTitle = item.albumTitle,
                Id = item.trackId,
                TrackTitle = item.title,
                DownloadUrl = UseMp3Fmt ? (string.IsNullOrWhiteSpace(item.playUrl64) ? item.playUrl32 ?? "" : item.playUrl64) :
                        string.IsNullOrWhiteSpace(item.playPathAacv224) ? item.playPathAacv164 ?? "" : item.playPathAacv224,
                Duration = Utils.GetTimeDur(item.duration)
            }));

            while (tracks.data.maxPageId > tracks.data.pageId)
            {
                tracks = await GetAlbumTrackList(albumID, ascFlg, ++tracks.data.pageId);

                if ((tracks?.data?.totalCount ?? 0) > 0)
                {
                    tracks.data.list.ForEach(item => data.Add(new TrackItem()
                    {
                        No = No++,
                        AlbumTitle = item.albumTitle,
                        Id = item.trackId,
                        TrackTitle = item.title,
                        DownloadUrl = UseMp3Fmt ? (string.IsNullOrWhiteSpace(item.playUrl64) ? item.playUrl32 ?? "" : item.playUrl64) :
                        string.IsNullOrWhiteSpace(item.playPathAacv224) ? item.playPathAacv164 ?? "" : item.playPathAacv224,
                        Duration = Utils.GetTimeDur(item.duration)
                    }));
                }
            }

            return data;
        }

        private async Task<GetAlbumInfoResponse> GetAlbumTrackList(string albumID, bool ascFlg, int pageId)
        {
            string url = $"https://mobile.ximalaya.com/mobile/v1/album/track/ts-{Utils.GetUnixTime()}?ac=WIFI&albumId={albumID}&device=android&isAsc={ascFlg}&pageId=1&pageSize=200";
            return await wa.Get(url);
        }

        public async Task Download(string url, string fileName, Action<long, long, bool> ReportProgess)
        {
            if (File.Exists(fileName))
            {
                FileInfo fs = new FileInfo(fileName);

                var contentsLen = await wa.GetContentsSize(url);

                if (contentsLen > 0 && contentsLen == fs.Length)
                {
                    return;
                }
                File.Delete(fileName);
            }

            await wa.DownloadFile(url, fileName, ReportProgess);
        }
    }
}
