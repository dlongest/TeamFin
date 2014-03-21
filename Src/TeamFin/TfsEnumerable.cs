using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TeamFin
{
    public class TfsEnumerable<TWorkItemType> : IEnumerable<TWorkItemType>, IEnumerable
        where TWorkItemType : TfsWorkItem
    {    
        private Expression _expression;
        private static IDictionary<string, string> _fields = new Dictionary<string, string>();
        private IWorkItemQueryStore<TWorkItemType> _store;

        public TfsEnumerable(IWorkItemQueryStore<TWorkItemType> store)
        {
            _store = store;
        }


        public TfsEnumerable(IWorkItemQueryStore<TWorkItemType> store, Expression expression)
        {
            if (store == null)
                throw new ArgumentNullException("workItemStore cannot be null");

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            this._store = store;
            this._expression = expression;
        }

      
        static TfsEnumerable()
        {
            var props = typeof(TWorkItemType)
                            .GetProperties()
                            .Where(a => Attribute.IsDefined(a, typeof(TfsFieldAttribute)));

            _fields = props.Select(a => new { PropertyName = a.Name, TfsFieldName = a.GetCustomAttributes(true).Cast<TfsFieldAttribute>().First().FieldName })
                .ToDictionary(a => a.PropertyName, b => b.TfsFieldName);
        }


        public virtual TfsEnumerable<TWorkItemType> Where(Expression<Func<TWorkItemType, bool>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression cannot be null");

            this._expression = (this._expression == null) ? expression : _expression.And<TWorkItemType>(expression);

            return this;
        }


        public IEnumerator<TWorkItemType> GetEnumerator()
        {
            var wiql = GetQuery();

            return ((IEnumerable<TWorkItemType>)this._store.Query(wiql)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var wiql = GetQuery();

            return ((IEnumerable)this._store.Query(wiql)).GetEnumerator();

        }           

        private string GetQuery()
        {
            return new TfsPredicateVisitor<TWorkItemType>(_fields).Translate(this._expression);
        }

        public override string ToString()
        {
            return GetQuery();
        }
    }
}
