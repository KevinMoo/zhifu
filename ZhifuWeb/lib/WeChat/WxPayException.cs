using System;

namespace ZhifuWeb.lib.WeChat
{
    public class WxPayException : Exception
    {
        public WxPayException(string msg)
            : base(msg)
        {
        }
    }
}