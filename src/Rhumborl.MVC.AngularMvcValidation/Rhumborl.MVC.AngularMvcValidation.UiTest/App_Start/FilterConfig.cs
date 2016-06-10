using System.Web;
using System.Web.Mvc;

namespace Rhumborl.MVC.AngularMvcValidation.UiTest
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
