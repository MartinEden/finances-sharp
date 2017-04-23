using FinancesSharp.Helpers;
using FinancesSharp.Models;
using FinancesSharp.Reports.Helpers;
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace FinancesSharp.Binders
{
    public abstract class Binder<TModel> : IModelBinder
    {
        private Type type;

        public Binder()
        {
            type = typeof(TModel);
        }

        public object BindModel(ControllerContext c, ModelBindingContext b)
        {
            if (b.Model == null)
            {
                b.ModelMetadata.Model = Activator.CreateInstance<TModel>();
            }
            bindProperties(c, b);
            return b.Model;
        }
        protected abstract void bindProperties(ControllerContext c, ModelBindingContext b);

        protected void bindProperty<T>(Expression<Func<TModel, T>> propertyExpression, ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string name = propertyExpression.GetPropertyName();
            var value = getValue<T>(name, bindingContext);
            setProperty<T>(bindingContext, name, value);
        }

        protected void bindNavigationProperty<T>(Expression<Func<TModel, T>> propertyExpression,
            ControllerContext controllerContext, ModelBindingContext bindingContext)
            where T : class, IHasId
        {
            string name = propertyExpression.GetPropertyName();
            int? id = getValue<int?>(name, bindingContext);
            if (id.HasValue)
            {
                T value = controllerContext.Db().Set<T>().SingleOrDefault(x => x.Id == id);
                setProperty(bindingContext, name, value);
            }
        }
        protected void bindDate(Expression<Func<TModel, DateTime?>> propertyExpression,
            ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string name = propertyExpression.GetPropertyName();
            string rawValue = getValue<string>(name, bindingContext);
            DateTime value;
            if (DateTime.TryParse(rawValue, out value))
            {
                setProperty<DateTime?>(bindingContext, name, value);
            }
        }
        protected void bindMonthAndYear(Expression<Func<TModel, MonthAndYear>> propertyExpression,
           ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string name = propertyExpression.GetPropertyName();
            string rawValue = getValue<string>(name, bindingContext);
            DateTime datetime;
            if (DateTime.TryParseExact(rawValue, "MMMM yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out datetime))
            {
                setProperty(bindingContext, name, new MonthAndYear(datetime));
            }
        }

        private static PropertyInfo getPropertyDescriptor(ModelBindingContext bindingContext, string name)
        {
            return bindingContext.ModelType.GetProperty(name);
        }
        private static void setProperty<T>(ModelBindingContext bindingContext, string name, T value)
        {
            getPropertyDescriptor(bindingContext, name).SetValue(bindingContext.Model, value, null);
        }

        private T getValue<T>(string name, ModelBindingContext bindingContext)
        {
            string fullName = ExpressionHelpers.Join(bindingContext.ModelName, name);
            var result = bindingContext.ValueProvider.GetValue(fullName);
            if (result != null && !string.IsNullOrEmpty(result.AttemptedValue))
            {
                return (T)result.ConvertTo(typeof(T));
            }
            return default(T);
        }
    }
}