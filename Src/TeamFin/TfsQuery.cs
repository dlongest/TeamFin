using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TeamFin
{
    public class TfsQuery<TWorkItemType> where TWorkItemType : TfsWorkItem 
    {         
        private readonly IWorkItemQueryStore<TWorkItemType> _store;
        private Expression _expression;   

        public TfsQuery(IWorkItemQueryStore<TWorkItemType> store)
        {
            _store = store;
        }

        internal TfsQuery(WorkItemStore workItemStore)
        {
            _store = new WorkItemQueryStore<TWorkItemType>(workItemStore);
        }
       

        public virtual TfsEnumerable<TWorkItemType> Where(Expression<Func<TWorkItemType, bool>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression cannot be null");

            this._expression = (this._expression == null) ? expression : _expression.And<TWorkItemType>(expression);

            return new TfsEnumerable<TWorkItemType>(_store, _expression);
           
        }

        public virtual TWorkItemType Single(Expression<Func<TWorkItemType, bool>> expression)
        {
            return new TfsEnumerable<TWorkItemType>(_store, expression).Single();
        }
    }

    internal static class ExpresionHelpers
    {
        public static Expression And<TWorkItemType>(this Expression first, Expression second) where TWorkItemType : TfsWorkItem
        {
            var mainBody = ((LambdaExpression)first).Body;
            var newBody = ((LambdaExpression)second).Body;

            var combinedExp = Expression.AndAlso(mainBody, newBody);
            var param = Expression.Parameter(typeof(TWorkItemType));
            return Expression.Lambda<Func<TWorkItemType, bool>>(combinedExp, param);
        }
    }       

}
