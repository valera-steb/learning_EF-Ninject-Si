using System.Web;
using System.Web.Mvc;

namespace EF_Ninject_Si
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
