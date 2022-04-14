using System;
using System.Collections.Generic;

namespace xmryDownloader
{
    public class GetAlbumInfoResponse
    {
        public string msg { get; set; }

        public int ret { get; set; }
        public AlbumInfoData data { get; set; }
    }

    public class AlbumInfoData
    {
        public int totalCount { get; set; }

        public int pageSize { get; set; }

        public int maxPageId { get; set; }

        public int pageId { get; set; }
        public List<TrackInfo> list { get; set; }
    }

    public class TrackInfo
    {
        public int albumId { get; set; }
        public string albumTitle { get; set; }

        public int duration { get; set; }

        public TimeSpan AudioDuration =>Utils.GetTimeDur(duration);

        public string playPathAacv224 { get; set; }

        public string playPathAacv164 { get; set; }

        public string playUrl32 { get; set; }

        public string playUrl64 { get; set; }


        public string title { get; set; }

    }
}
