using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ZhifuWeb.lib.WeChat
{
    public class Jssdk
    {
        public static AccessTokenResult GetToken(string appid, string secret, string grantType = "client_credential")
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type={0}&appid={1}&secret={2}",
                                    grantType, appid, secret);

            AccessTokenResult result = Get.GetJson<AccessTokenResult>(url);

            //cache.Insert("AccessToken" + appid, result, null, DateTime.Now.AddHours(1.5), Cache.NoSlidingExpiration);
            return result;
        }

        public static JsApiTicketResult GetJsApi_Ticket(string accessToken, string appid)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi",
                                    accessToken);
            JsApiTicketResult result = Get.GetJson<JsApiTicketResult>(url);
            return result;
        }

        public static WxConfigResult GetWxConfigResult(string appid, string secret, string url)
        {
            url = url.IndexOf("#", StringComparison.Ordinal) >= 0 ? url.Substring(0, url.IndexOf("#", StringComparison.Ordinal)) : url;
            var jsapiTicket = GetJsApi_Ticket(GetToken(appid, secret).access_token, appid).ticket;
            var timestamp = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            var noncestr = Function.GuidTo16String(Guid.NewGuid());

            var str = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", jsapiTicket, noncestr, timestamp, url);

            var hash = Function.Sha1(str);

            var wxConfigResult = new WxConfigResult()
            {
                appid = appid,
                timestamp = timestamp.ToString(),
                noncestr = noncestr,
                signature = hash,
                ticket = jsapiTicket,
                url = str
            };
            return wxConfigResult;
        }
    }

    public class AccessTokenResult
    {
        /// <summary>
        ///     获取到的凭证
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        ///     凭证有效时间，单位：秒
        /// </summary>
        public int expires_in { get; set; }
    }

    public class JsApiTicketResult
    {
        public string errorcode { get; set; }

        public string errormsg { get; set; }

        public string ticket { get; set; }

        public string vsdshFKA { get; set; }

        public string expires_in { get; set; }
    }

    public static class Function
    {
        public static string GuidTo16String(Guid ID)
        {
            long i = 1;
            foreach (byte b in ID.ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        public static string Sha1(string str)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] dataToHash = enc.GetBytes(str);
            //Hash运算
            byte[] dataHashed = sha1.ComputeHash(dataToHash);
            //将运算结果转换成string
            string hash = BitConverter.ToString(dataHashed).Replace("-", "").ToLower();
            return hash;
        }
    }

    public class WxJsonResult
    {
        public ReturnCode errcode { get; set; }

        public string errmsg { get; set; }

        /// <summary>
        /// 为P2P返回结果做准备
        /// </summary>
        public object data { get; set; }
    }

    public static class Get
    {
        public static T GetJson<T>(string url)
        {
            string returnText = HttpService.Get(url);
            if (returnText.Contains("errcode"))
            {
                //可能发生错误
                WxJsonResult errorResult = JsonConvert.DeserializeObject<WxJsonResult>(returnText);
                if (errorResult.errcode != ReturnCode.请求成功)
                {
                    //发生错误
                    throw new Exception(
                        string.Format("微信请求发生错误！错误代码：{0}，说明：{1},errorResult:{2}",
                                        (int)errorResult.errcode,
                                        errorResult.errmsg, errorResult));
                }
            }

            T result = JsonConvert.DeserializeObject<T>(returnText);

            return result;
        }
    }

    public enum ReturnCode
    {
        系统繁忙 = -1,
        请求成功 = 0,
        验证失败 = 40001,
        不合法的凭证类型 = 40002,
        不合法的OpenID = 40003,
        不合法的媒体文件类型 = 40004,
        不合法的文件类型 = 40005,
        不合法的文件大小 = 40006,
        不合法的媒体文件id = 40007,
        不合法的消息类型 = 40008,
        不合法的图片文件大小 = 40009,
        不合法的语音文件大小 = 40010,
        不合法的视频文件大小 = 40011,
        不合法的缩略图文件大小 = 40012,
        不合法的APPID = 40013,

        //不合法的access_token      =             40014,
        不合法的access_token = 40014,

        不合法的菜单类型 = 40015,

        //不合法的按钮个数             =          40016,
        //不合法的按钮个数              =         40017,
        不合法的按钮个数1 = 40016,

        不合法的按钮个数2 = 40017,
        不合法的按钮名字长度 = 40018,
        不合法的按钮KEY长度 = 40019,
        不合法的按钮URL长度 = 40020,
        不合法的菜单版本号 = 40021,
        不合法的子菜单级数 = 40022,
        不合法的子菜单按钮个数 = 40023,
        不合法的子菜单按钮类型 = 40024,
        不合法的子菜单按钮名字长度 = 40025,
        不合法的子菜单按钮KEY长度 = 40026,
        不合法的子菜单按钮URL长度 = 40027,
        不合法的自定义菜单使用用户 = 40028,
        缺少access_token参数 = 41001,
        缺少appid参数 = 41002,
        缺少refresh_token参数 = 41003,
        缺少secret参数 = 41004,
        缺少多媒体文件数据 = 41005,
        缺少media_id参数 = 41006,
        缺少子菜单数据 = 41007,
        access_token超时 = 42001,
        需要GET请求 = 43001,
        需要POST请求 = 43002,
        需要HTTPS请求 = 43003,
        多媒体文件为空 = 44001,
        POST的数据包为空 = 44002,
        图文消息内容为空 = 44003,
        多媒体文件大小超过限制 = 45001,
        消息内容超过限制 = 45002,
        标题字段超过限制 = 45003,
        描述字段超过限制 = 45004,
        链接字段超过限制 = 45005,
        图片链接字段超过限制 = 45006,
        语音播放时间超过限制 = 45007,
        图文消息超过限制 = 45008,
        接口调用超过限制 = 45009,
        创建菜单个数超过限制 = 45010,
        不存在媒体数据 = 46001,
        不存在的菜单版本 = 46002,
        不存在的菜单数据 = 46003,
        解析JSON_XML内容错误 = 47001
    }
}