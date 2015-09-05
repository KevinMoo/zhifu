namespace ZhifuWeb.lib.WeChat
{
    public class PrePayModel
    {
        public string AppId { get; set; }

        public string NonceStr { get; set; }

        public string PackAge { get; set; }

        public string PaySign { get; set; }

        public string SignType { get; set; }

        public string TimeStamp { get; set; }
    }
}