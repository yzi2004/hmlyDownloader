using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xmryDownloader
{
    public class GetAlbumInfoResponse
    {
        public string costTime { get; set; }
        public AlbumInfoData data { get; set; }
    }

    public class AlbumInfoData
    {
        public AlbumInfo album { get; set; }
    }

    public class AlbumInfo
    {
        public int albumId { get; set; }
        public long createdAt { get; set; }

        public string title { get; set; }

        public int totalTrackCount { get; set; }

        public int tracks { get; set; }
    }
}
