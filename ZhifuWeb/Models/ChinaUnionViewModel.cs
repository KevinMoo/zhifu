using System;
using System.Web;
using ZhifuWeb.lib.ChinaUnion;

namespace ZhifuWeb.Models
{
    public class ChinaUnionViewModel
    {
        public ChinaUnionViewModel()
        {
            Version = "5.0.0";
            Encoding = "UTF-8";
            CertId = CertUtil.GetSignCertId();
            SignMethod = "01";
            TxnType = "31";
            TxnSubType = "00";
            BizType = "000201";
            AccessType = "0";
            ChannelType = "07";
            TxnTime = DateTime.Now.ToString("yyyyMMddHHmmss");

            MerId = "898110279910127";
            BackUrl = "http://" + HttpContext.Current.Request.Url.Host + "/ChinaUnion/BackReceive";
            FrontUrl = "http://" + HttpContext.Current.Request.Url.Host + "/ChinaUnion/FrontUrl";
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 编码方式
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// 证书ID
        /// </summary>
        public string CertId { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        public string TxnType { get; set; }

        /// <summary>
        /// 交易子类
        /// </summary>
        public string TxnSubType { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BizType { get; set; }

        /// <summary>
        /// 前台通知地址
        /// </summary>
        public string FrontUrl { get; set; }

        /// <summary>
        /// 后台通知地址，改自己的外网地址
        /// </summary>
        public string BackUrl { get; set; }

        /// <summary>
        /// 签名方法
        /// </summary>
        public string SignMethod { get; set; }

        /// <summary>
        /// 渠道类型，07-PC，08-手机
        /// </summary>
        public string ChannelType { get; set; }

        /// <summary>
        /// 接入类型
        /// </summary>
        public string AccessType { get; set; }

        /// <summary>
        /// 商户号，请改成自己的商户号
        /// </summary>
        public string MerId { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 订单发送时间
        /// </summary>
        public string TxnTime { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        public string TxnAmt { get; set; }

        /// <summary>
        /// 交易币种，单位分
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// 订单描述
        /// </summary>
        public string OrderDesc { get; set; }

        /// <summary>
        /// 请求方保留域，透传字段，查询、通知、对账文件中均会原样出现
        /// </summary>
        public string ReqReserved { get; set; }
    }
}