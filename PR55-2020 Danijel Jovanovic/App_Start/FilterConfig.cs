using System.Web;
using System.Web.Mvc;

namespace PR55_2020_Danijel_Jovanovic
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
