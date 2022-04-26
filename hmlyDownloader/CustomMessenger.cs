namespace hmlyDownloader
{
    public class CustomMessenger
    {
        public enum ACTION
        {
            showmsg,
            selfolder,
            setfocus,
            scrollview
        };

        public ACTION action { get; set; }

        public string message { get; set; }

        public string result { get; set; }
    }
}
