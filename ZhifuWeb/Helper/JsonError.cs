namespace ZhifuWeb.Helper
{
    public class JsonError
    {
        public string errorcode { get; set; }

        public string message { get; set; }
    }

    public static class Helper
    {
        public static string SetNull(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            return value;
        }
    }
}