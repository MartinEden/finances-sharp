using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace FinancesSharp.Helpers
{
    public static class ExpressionHelpers
    {
        public static string GetPropertyName<TModel, TProperty>(this Expression<Func<TModel, TProperty>> lambda)
        {
            string result = GetPropertyName(lambda.Body);
            if (result == null)
            {
                throw new InvalidPropertyExpressionException(lambda);
            }
            return result;
        }

        private static string GetPropertyName(Expression expression)
        {
            MemberExpression member = expression as MemberExpression;
            if (member == null)
                return null;

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                return null;

            string parent = GetPropertyName(member.Expression);
            return Join(parent, propInfo.Name);
        }

        public static string Join(string a, string b)
        {
            if (a == null && b == null)
            {
                return null;
            }
            else if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b))
            {
                return a + b;
            }
            else
            {
                return a + "." + b;
            }
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {
            ParameterExpression p = a.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor.subst[b.Parameters[0]] = p;

            Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {
            ParameterExpression p = a.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor.subst[b.Parameters[0]] = p;

            Expression body = Expression.OrElse(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }
    }

    public class InvalidPropertyExpressionException : Exception
    {
        public readonly Expression Lambda;

        private const string message =
            "The expression '{0}' was not a property accessor expression. " +
            "It must be in the form x => x.a, x => x.a.b, etc., where 'a' and 'b' properties, not fields or methods.";

        public InvalidPropertyExpressionException(Expression lambda)
            : base(string.Format(message, lambda))
        {
            this.Lambda = lambda;
        }
    }
}