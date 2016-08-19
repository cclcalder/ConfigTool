using System;
using System.Linq.Expressions;

namespace Exceedra.Common
{
    public static class ReflectionUtils
    {
        public static string GetPropertyName<T>(Expression<Func<T, object>> exp)
        {
            var body = exp.Body as MemberExpression;

            if (body == null)
            {
                var ubody = (UnaryExpression)exp.Body;
                body = (MemberExpression)ubody.Operand;
            }

            return body.Member.Name;
        }
    }
}