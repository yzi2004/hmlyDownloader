using System;
using System.IO;
using System.Threading.Tasks;

namespace xmryDownloader
{
    internal class xmryDownloadService
    {
        WebAccess wa = new WebAccess();

        public async Task<TrackViewModel> GetAlbumTrackList(string albumID, bool ascFlg = true)
        {
            TrackViewModel vm = new TrackViewModel();
            var tracks = await GetAlbumTrackList(albumID, ascFlg, 1);

            if (tracks == null || tracks.ret != 0)
            {
                vm.ErrMsg = string.IsNullOrWhiteSpace(tracks?.msg) ? "Noops,系统出错了。" : tracks.msg;
                return vm;
            }
            if ((tracks.data?.totalCount ?? 0) <= 0)
            {
                vm.ErrMsg = "Oh,Nooo, 为啥找不到音频文件呢。";
                return vm;
            }

            int No = 1;

            vm.Title = tracks.data.list[0].albumTitle;
            vm.TrackCount = tracks.data.totalCount;
            tracks.data.list.ForEach(item => vm.Data.Add(new TrackItem()
            {
                No = No++,
                Id = item.trackId,
                Title = item.title,
                M4aUrl = string.IsNullOrWhiteSpace(item.playPathAacv224) ? item.playPathAacv164 ?? "" : item.playPathAacv224,
                Mp3Url = string.IsNullOrWhiteSpace(item.playUrl64) ? item.playUrl32 ?? "" : item.playUrl64,
                Duration = Utils.GetTimeDur(item.duration)
            }));

            while (tracks.data.maxPageId > tracks.data.pageId)
            {
                tracks = await GetAlbumTrackList(albumID, ascFlg, ++tracks.data.pageId);

                if ((tracks?.data?.totalCount ?? 0) > 0)
                {
                    tracks.data.list.ForEach(item => vm.Data.Add(new TrackItem()
                    {
                        No = No++,
                        Id = item.trackId,
                        Title = item.title,
                        M4aUrl = string.IsNullOrWhiteSpace(item.playPathAacv224) ? item.playPathAacv164 ?? "" : item.playPathAacv224,
                        Mp3Url = string.IsNullOrWhiteSpace(item.playUrl64) ? item.playUrl32 ?? "" : item.playUrl64,
                        Duration = Utils.GetTimeDur(item.duration)
                    })); ;
                }
            }

            return vm;
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
                FileInfo fs  = new FileInfo(fileName);

                var contentsLen = await wa.GetContentsSize(url);

                if(contentsLen >0 && contentsLen  == fs.Length)
                {
                    return;
                }
                File.Delete(fileName);
            }

            await wa.DownloadFile(url, fileName, ReportProgess);
        }
    }
}
