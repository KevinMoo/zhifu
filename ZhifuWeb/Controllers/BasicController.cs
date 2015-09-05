using Hzsp.Helper;
using System;
using System.Web.Mvc;

namespace ZhifuWeb.Controllers
{
    public class BasicController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
                base.OnException(filterContext);

            LogException(filterContext.Exception);
            if (filterContext.HttpContext.IsCustomErrorEnabled)
            {
                filterContext.ExceptionHandled = true;
                this.View("Error").ExecuteResult(this.ControllerContext);
            }
        }

        protected virtual void LogException(Exception ex)
        {
            LogHelper.WriteLog(ex);
        }
    }
}