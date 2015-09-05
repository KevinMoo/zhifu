using Hzsp.Helper;
using System.Web.Mvc;

namespace ZhifuWeb.Filter
{
    public class ExceptionFilterAttribute : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                LogHelper.WriteLog(filterContext.Exception);
                filterContext.ExceptionHandled = true;
                throw filterContext.Exception;
            }
        }
    }
}