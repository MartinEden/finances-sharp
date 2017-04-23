using FinancesSharp.ViewModels;
using System;
using System.Linq.Expressions;
using System.Web.Routing;

namespace FinancesSharp.Helpers
{
    public static class RouteValueDictionaryExtensions
    {
        public static void Add<TModel, TValue>(this RouteValueDictionary dict, TModel model, Expression<Func<TModel, TValue>> property)
        {
            string propertyName = property.GetPropertyName();
            object value = property.Compile()(model);
            dict.Add(propertyName, value);
        }
        public static void Add<TModel, TValue>(this RouteValueDictionary dict, TModel model, Expression<Func<TModel, TValue>> property, object value)
        {
            string propertyName = property.GetPropertyName();
            dict.Add(propertyName, value);
        }

        public static void AddSubDictionary<TModel>(this RouteValueDictionary dict, TModel model, 
            Expression<Func<TModel, IConvertibleToRouteValues>> property)
        {
            string prefix = property.GetPropertyName();
            var subDict = property.Compile()(model).ToRouteValues();
            foreach (var kv in subDict)
            {
                dict.Add(ExpressionHelpers.Join(prefix, kv.Key), kv.Value);
            }            
        }
    }
}