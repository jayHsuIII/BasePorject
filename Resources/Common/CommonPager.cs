using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using System.Linq.Expressions;

namespace Resources.Common
{
    /// <summary>
    /// 2017.01.26_JAY
    /// </summary>
    public static class CommonPager<TEntity> where TEntity : class
    {
        /// <summary>
        /// 取得換頁List資料
        /// </summary>
        /// <param name="queryData">IQueryable的資料</param>
        /// <param name="pager">換頁參數物件</param>
        /// <returns></returns>
        public static List<TEntity> ls_GetData(IQueryable<TEntity> queryData, Pager<TEntity> pager)
        {
            IQueryable<TEntity> IQueryableData = IQueryableExtension.OrderBy<TEntity>(queryData);

            //取得IPagedList
            pager.pagerData = IQueryableData.ToPagedList(pager.pageNumber, pager.pageSize);
            return IQueryableData.ToPagedList(pager.pageNumber, pager.pageSize).ToList();
        }

        /// <summary>
        /// 自行決定排序用
        /// </summary>
        /// <param name="queryData">必須是order by 過後的資料</param>
        public static List<TEntity> ls_GetDataNoAutoOrder(IQueryable<TEntity> queryData, Pager<TEntity> pager)
        {
            //取得IPagedList
            pager.pagerData = queryData.ToPagedList(pager.pageNumber, pager.pageSize);
            return queryData.ToPagedList(pager.pageNumber, pager.pageSize).ToList();
        }

    }

    /// <summary>
    /// 泛型Order用，提供pageList.MVC使用
    /// </summary>
    public static class IQueryableExtension
    {
        private static MethodInfo orderbyInfo = null;
        private static MethodInfo orderbyDecInfo = null;

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string property = "") where T : class
        {
            Type entityType = typeof(T);
            if (property == "")
            {
                property = entityType.GetProperties()[0].Name;
            }
            Type entityPropertyType = entityType.GetProperty(property).PropertyType;

            Type tempType;//type物件
            string tempProperty = "";//屬性名稱
            List<string> propertyList = new List<string>();
            propertyList.Add(property);
            //經觀察如果Namespace!=System表示還沒到最深層，要繼續增加Lamdba鍊式串接_2017.02.07_JAY
            while (entityPropertyType.Namespace != "System")
            {
                tempProperty = entityPropertyType.GetProperties()[0].Name;
                tempType = entityPropertyType.GetProperty(tempProperty).PropertyType;
                entityPropertyType = tempType;
                propertyList.Add(tempProperty);
            }

            //動態產生Lamdba語法
            var orderPara = Expression.Parameter(entityType, "o");
            MemberExpression tempMemberExpression = Expression.Property(orderPara, propertyList[0]);
            for (int i = 1; i < propertyList.Count; i++)
            {
                MemberExpression temp = Expression.Property(tempMemberExpression, propertyList[i]);
                tempMemberExpression = temp;
            }
            var orderExpr = Expression.Lambda(tempMemberExpression, orderPara);

            if (orderbyInfo == null)
            {
                //因為呼叫OrderBy需要知道型別，不知道的情況下無法直接呼叫，所以用反射的方式呼叫
                //泛型的GetMethod很難，所以用GetMethods在用Linq取出Method，找到後快取。
                orderbyInfo = typeof(Queryable).GetMethods().Single(x => x.Name == "OrderBy" && x.GetParameters().Length == 2);
            }

            //因為是泛型Mehtod要呼叫MakeGenericMethod決定泛型型別
            return orderbyInfo.MakeGenericMethod(new Type[] { entityType, entityPropertyType }).Invoke(null, new object[] { query, orderExpr }) as IQueryable<T>;
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string property)
        {
            Type entityType = typeof(T);
            Type entityPropertyType = entityType.GetProperty(property).PropertyType;

            var orderPara = Expression.Parameter(entityType, "o");
            var orderExpr = Expression.Lambda(Expression.Property(orderPara, property), orderPara);

            if (orderbyDecInfo == null)
            {
                orderbyDecInfo = typeof(Queryable).GetMethods().Single(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 2);
            }

            return orderbyDecInfo.MakeGenericMethod(new Type[] { entityType, entityPropertyType }).Invoke(null, new object[] { query, orderExpr }) as IQueryable<T>;
        }
    }

    /// <summary>
    /// 2017.01.26_JAY
    /// </summary>
    public class Pager<T> where T : class
    {
        public Pager()
        {
            this.pageNumber = 1;
            this.pageSize = 6;
        }

        /// <summary>
        /// 第幾頁
        /// </summary>
        public int pageNumber { get; set; }

        /// <summary>
        /// 一頁有多少內容
        /// </summary>
        public int pageSize { get; set; }

        /// <summary>
        /// 換頁用
        /// </summary>
        public IPagedList<T> pagerData { get; set; }
    }
}
