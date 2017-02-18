using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Resources.Common
{
    /// <summary>
    /// 支援函式
    /// 20170124_JAY
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// 函數對應True時：單個And有效，多個And有效；單個OR無效，多個OR無效；混和應用時寫在AND後面的OR有效  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        /// <summary>
        /// 函數對應False時：單個AND無效，多個AND無效；單個OR有效，多個OR有效；混和應用時寫在OR後面的AND有效  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        //注意this
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.Or(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                             Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.And(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }
}
