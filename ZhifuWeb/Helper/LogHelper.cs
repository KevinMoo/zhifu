using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;

namespace Hzsp.Helper
{
    public static class LogHelper
    {
        /// <summary>
        /// 订单处理日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="orderId">订单号（作为日志文件名称）</param>
        public static void CreateLog(string message, string orderId)
        {
            if (orderId == "wxdebug")
            {
                if (ConfigurationManager.AppSettings["TestWeixin"] != "true")
                    return;
            }
            try
            {
                string path = HttpContext.Current.Server.MapPath("/logs/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = orderId + ".txt";
                using (var fs = new FileStream(path + fileName, FileMode.Append, FileAccess.Write, FileShare.Write))
                {
                    string str = "|---------------------------------------------------------------|" + Environment.NewLine;
                    str += "|--------------------" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "--------------------|" + Environment.NewLine;
                    str += "|---------------------------------------------------------------|" + Environment.NewLine;
                    str += message;
                    var bytes = Encoding.Default.GetBytes(str);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex);
            }
        }

        /// <summary>
        /// 记录普通错误
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="path"></param>
        /// <param name="fileName">默认是当前时间+guid</param>
        public static void WriteLog(Exception ex, string path = @"/logs/GeneralLog", string fileName = "")
        {
            if (!path.Contains("\\")) //相对路径
            {
                path = HttpContext.Current.Server.MapPath(path);
            }
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!path.EndsWith("\\"))
                path += "\\";
            try
            {
                using (var fs = new FileStream(path + fileName, FileMode.Append, FileAccess.Write, FileShare.Write))
                {
                    string str = "|---------------------------------------------------------------|" +
                                 Environment.NewLine;
                    str += "|--------------------" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                           "--------------------|" + Environment.NewLine;
                    str += "|---------------------------------------------------------------|" + Environment.NewLine;
                    str += JsonConvert.SerializeObject(ex);
                    var bytes = Encoding.Default.GetBytes(str);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 记录普通错误
        /// </summary>
        /// <param name="message"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        public static void WriteLog(string message, string path, string fileName = "")
        {
            if (!path.Contains("\\")) //相对路径
            {
                path = HttpContext.Current.Server.MapPath(path);
            }
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!path.EndsWith("\\"))
                path += "\\";
            using (var fs = new FileStream(path + fileName, FileMode.Append, FileAccess.Write, FileShare.Write))
            {
                string str = "|---------------------------------------------------------------|" + Environment.NewLine;
                str += "|--------------------" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "--------------------|" + Environment.NewLine;
                str += "|---------------------------------------------------------------|" + Environment.NewLine;
                str += message;
                var bytes = Encoding.Default.GetBytes(str);
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
            }
        }

        public static void WriteHtml(string message, string path, string fileName = "")
        {
            if (!path.Contains("\\")) //相对路径
            {
                path = HttpContext.Current.Server.MapPath(path);
            }
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".html";
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!path.EndsWith("\\"))
                path += "\\";
            using (StreamWriter sw = new StreamWriter(path + fileName, true, Encoding.UTF8))
            {
                sw.WriteLine("<html>");
                sw.WriteLine("<head>");
                sw.WriteLine("<meta charset='utf-8'>");
                sw.WriteLine("</head>");
                sw.WriteLine("<body>");
                sw.WriteLine(message);
                sw.WriteLine("</body>");
                sw.WriteLine("</body>");
                sw.WriteLine("</html>");
            }
        }
    }
}