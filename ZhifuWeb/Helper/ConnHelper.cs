using ZhifuWeb.EF;

namespace ZhifuWeb.Helper
{
    public static class ConnHelper
    {
        public static ZhifuDb CreateDb()
        {
            ZhifuDb context = new ZhifuDb();
            return context;
        }
    }
}