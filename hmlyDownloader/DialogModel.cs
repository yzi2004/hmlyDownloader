namespace hmlyDownloader
{
    public class DialogModel
    {
        public enum ACTION
        {
            showmsg,
            selfolder
        };

        public ACTION action { get; set; }

        public string message { get; set; }

        public string result { get; set; }
    }
}
