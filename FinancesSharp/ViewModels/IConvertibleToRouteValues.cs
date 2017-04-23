using System.Web.Routing;

namespace FinancesSharp.ViewModels
{
    public interface IConvertibleToRouteValues
    {
        RouteValueDictionary ToRouteValues();
    }
}
