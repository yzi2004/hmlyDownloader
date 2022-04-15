using System;
using System.Collections.Generic;

namespace xmryDownloader
{
    internal class TrackViewModel
    {
        public string Title { get; set; }

        public int TrackCount { get; set; }

        public string ErrMsg { get; set; }

        public List<TrackItem> Data { get; set; } = new List<TrackItem>();
    }

    internal class TrackItem
    {
        public int No { get; set; }

        public int Id { get; set; }

        public string Title { get; set; }

        public TimeSpan Duration { get; set; }

        public string Mp3Url { get; set; }

        public string M4aUrl { get; set; }
    }
}
