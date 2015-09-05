using System.Configuration;
using System.Web;
using System.Web.Configuration;

namespace ZhifuWeb.lib.ChinaUnion
{
    public class SDKConfig
    {
        private static Configuration config = WebConfigurationManager.OpenWebConfiguration("~");

        private static string signCertPath = HttpContext.Current.Server.MapPath(config.AppSettings.Settings["ChinaUnion_signCertPath"].Value);  //功能：读取配置文件获取签名证书路径
        private static string signCertPwd = config.AppSettings.Settings["ChinaUnion_signCert.pwd"].Value;//功能：读取配置文件获取签名证书密码
        private static string validateCertDir = HttpContext.Current.Server.MapPath(config.AppSettings.Settings["ChinaUnion_validateCertDir"].Value);//功能：读取配置文件获取验签目录
        public static string encryptCert = HttpContext.Current.Server.MapPath(config.AppSettings.Settings["ChinaUnion_encryptCertPath"].Value);  //功能：加密公钥证书路径
        private static string cardRequestUrl = config.AppSettings.Settings["ChinaUnion_cardRequestUrl"].Value;  //功能：有卡交易路径;
        private static string appRequestUrl = config.AppSettings.Settings["ChinaUnion_appRequestUrl"].Value;  //功能：appj交易路径;

        private static string singleQueryUrl = config.AppSettings.Settings["ChinaUnion_singleQueryUrl"].Value; //功能：读取配置文件获取交易查询地址
        private static string fileTransUrl = config.AppSettings.Settings["ChinaUnion_fileTransUrl"].Value;  //功能：读取配置文件获取文件传输类交易地址
        private static string frontTransUrl = config.AppSettings.Settings["ChinaUnion_frontTransUrl"].Value; //功能：读取配置文件获取前台交易地址
        private static string backTransUrl = config.AppSettings.Settings["ChinaUnion_backTransUrl"].Value;//功能：读取配置文件获取后台交易地址
        private static string batTransUrl = config.AppSettings.Settings["ChinaUnion_batTransUrl"].Value;//功能：读取配批量交易地址

        public static string CardRequestUrl
        {
            get { return SDKConfig.cardRequestUrl; }
            set { SDKConfig.cardRequestUrl = value; }
        }

        public static string AppRequestUrl
        {
            get { return SDKConfig.appRequestUrl; }
            set { SDKConfig.appRequestUrl = value; }
        }

        public static string FrontTransUrl
        {
            get { return SDKConfig.frontTransUrl; }
            set { SDKConfig.frontTransUrl = value; }
        }

        public static string EncryptCert
        {
            get { return SDKConfig.encryptCert; }
            set { SDKConfig.encryptCert = value; }
        }

        public static string BackTransUrl
        {
            get { return SDKConfig.backTransUrl; }
            set { SDKConfig.backTransUrl = value; }
        }

        public static string SingleQueryUrl
        {
            get { return SDKConfig.singleQueryUrl; }
            set { SDKConfig.singleQueryUrl = value; }
        }

        public static string FileTransUrl
        {
            get { return SDKConfig.fileTransUrl; }
            set { SDKConfig.fileTransUrl = value; }
        }

        public static string SignCertPath
        {
            get { return SDKConfig.signCertPath; }
            set { SDKConfig.signCertPath = value; }
        }

        public static string SignCertPwd
        {
            get { return SDKConfig.signCertPwd; }
            set { SDKConfig.signCertPwd = value; }
        }

        public static string ValidateCertDir
        {
            get { return SDKConfig.validateCertDir; }
            set { SDKConfig.validateCertDir = value; }
        }

        public static string BatTransUrl
        {
            get { return SDKConfig.batTransUrl; }
            set { SDKConfig.batTransUrl = value; }
        }
    }
}