using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TeamFin
{
    /// <summary>
    /// Generates the WorkItem SQL (WIQL) from a given Expressoin that is used by the IWorkItemQueryStore to retrieve
    /// work items.  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TfsPredicateVisitor<T>
    {       
        private StringBuilder wiql;
        private static PropertyInfo[] _enumProperties;
        private static Stack<PropertyInfo> _currentEnum = new Stack<PropertyInfo>();
        private static IDictionary<string, string> _fields = new Dictionary<string, string>();

        /// <summary>
        /// Finds all the enumerated types in T and stores them in _enumProperties. 
        /// </summary>
        static TfsPredicateVisitor()
        {
            _enumProperties = typeof(T).GetProperties().Where(a => a.PropertyType.IsEnum).ToArray();
        }

        /// <summary>
        /// Creates the PredicateVisitor using the provided fieldMappings.  Field Mappings use a key of property names
        /// of T to a value of the TFS schema column name for that property. Throws ArgumentNullException if 
        /// fieldMappings is null.
        /// </summary>
        /// <param name="fieldMappings"></param>
        public TfsPredicateVisitor(IDictionary<string, string> fieldMappings)
        {
            if (fieldMappings == null)
                throw new ArgumentNullException("fieldMappings cannot be null");

            _fields = fieldMappings;
        }

        /// <summary>
        /// Returns true if provided Expression is a Func of T, bool; false otherwise. 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public bool CanTranslate(Expression exp)
        {
            return (exp as Expression<Func<T, bool>>) != null;
        }

        /// <summary>
        /// Returns the WIQL for the given Expression.  Throws NotSupportedExceptoin if Expression is not an 
        /// Expression of Func of T, bool.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public string Translate(Expression exp)
        {
            if (!CanTranslate(exp))
                throw new NotSupportedException("This class is only able to translate Expression<Func<T, bool>>");

            return Translate(exp as Expression<Func<T, bool>>);
        }

        /// <summary>
        /// Returns the WIQL for the given Expression. 
        /// </summary>
        /// <param name="pred"></param>
        /// <returns></returns>
        public string Translate(Expression<Func<T,bool>> pred)
        {
            if (pred == null)
                throw new ArgumentNullException("Predicate cannot be null");

            wiql = new StringBuilder();
            wiql.Append("SELECT * From WorkItems WHERE ");
            
            Visit(pred);

            return wiql.ToString();
        }


        private Expression Visit(Expression exp)
        {
            if (exp == null)
                return exp;

            switch (exp.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return VisitBinary(exp as BinaryExpression);
                case ExpressionType.Lambda:
                    return VisitLambda(exp as LambdaExpression);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);                  
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Convert:
                    return this.VisitConvert((UnaryExpression)exp);
                case ExpressionType.Not:
                    return this.VisitUnary((UnaryExpression)exp);       
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                default:
                    throw new ArgumentException("Expression is not supported");
            }

        }

        private Expression VisitLambda(LambdaExpression exp)
        {
            return Visit(exp.Body);
        }



        protected virtual Expression VisitBinary(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    wiql.Append("(");
                    Visit(b.Left);
                    wiql.Append(" AND ");
                    Visit(b.Right);
                    wiql.Append(")");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    wiql.Append("(");
                    Visit(b.Left);
                    wiql.Append(" OR ");
                    Visit(b.Right);
                    wiql.Append(")");
                    break;
                case ExpressionType.Equal:
                    Visit(b.Left);
                    wiql.Append(" = ");
                    Visit(b.Right);
                    break;
                case ExpressionType.NotEqual:
                    Visit(b.Left);
                    wiql.Append(" <> ");
                    Visit(b.Right);
                    break;
                case ExpressionType.LessThan:
                    Visit(b.Left);
                    wiql.Append(" < ");
                    Visit(b.Right);
                    break;
                case ExpressionType.LessThanOrEqual:
                    Visit(b.Left);
                    wiql.Append(" <= ");
                    Visit(b.Right);
                    break;
                case ExpressionType.GreaterThan:
                    Visit(b.Left);
                    wiql.Append(" > ");
                    Visit(b.Right);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    Visit(b.Left);
                    wiql.Append(" >= ");
                    Visit(b.Right);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }
            return b;
        }

        
        protected virtual Expression VisitConvert(UnaryExpression u)
        {
            var opType = u.Operand.Type;

            // If the operand is an enum
            if (_enumProperties.Select(a => a.PropertyType).Contains(opType))
            {
                var prop = _enumProperties.First(a => a.PropertyType == opType);
                VisitMemberAccess((MemberExpression)u.Operand);
                _currentEnum.Push(prop);
            }
            else
            {
                VisitUnary(u);
            }

            return u;
        }

        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            if (u.NodeType == ExpressionType.Not)
            {                
                wiql.Append(" not ");
                Visit(u.Operand);
            }

            return u;
        }


        protected virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }


      
        private Expression VisitMemberAccess(MemberExpression m)
        {
            // The member expression can be either a parameter of the lambda expression OR it can be a closure variable captured
            // by the lambda.  Example:
            // a => a.Id == id
            // a.Id is a MemberExpression with NodeType of ExpressionType.Parameter
            // id is a MemberExpression holding a bound variable with NodeType of ExpressionType.Constant
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                var fieldName = _fields[m.Member.Name];

                wiql.Append(fieldName);
                return m;
            }
            else if (m.Expression != null && m.Expression.NodeType == ExpressionType.Constant)
            {
                //  Have to compile the expression and then invoke it to actually get the value from it 
                var value = Expression.Lambda(m).Compile().DynamicInvoke();

                wiql.Append(StringifyForAppend(value));

                return m;
            }
            else if (m.Expression == null)
            {
                var member = m.Member;            

                wiql.Append(StringifyForAppend(Expression.Lambda(m).Compile().DynamicInvoke()));

                return m;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        protected virtual Expression VisitConstant(ConstantExpression c)
        {
            // If the last processed expression was an enum, take this value, cast as that enum, and get its 
            // string representation.  Otherwise, append it as normal.  Use a single quote around strings, which
            // includes both the string representation of an enum as well as a string value in the ConstantExpression                                 

            if (_currentEnum.Any())
            {
                var current = _currentEnum.Pop();

                var enumString = Enum.GetName(current.PropertyType, c.Value);

                wiql.Append("'" + enumString + "'");
            }
            else
            {
                wiql.Append(StringifyForAppend(c.Value));
            }

            return c;
        }

        /// <summary>
        /// If the MethodCallExpression is Contains, StartsWith, or EndsWith on type string, converts it
        /// into the equivalent WIQL clause.  Throws NotSupportException for all other methods (either 
        /// on the string type or on any type). 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType != typeof(string))
                throw new NotSupportedException("Only methods against strings are supported");

            if (m.Method.Name == "Contains" || m.Method.Name == "StartsWith" || m.Method.Name == "EndsWith")
            {
                Visit(m.Object);

                wiql.Append(" contains ");

                var searchStringExpression = m.Arguments[0];

                Visit(searchStringExpression);
            }

            return m;
        }

        /// <summary>
        /// If o is of type string, quotes its string representation.  Otherwise, returns o's string representation
        /// without surrounding quotes.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private string StringifyForAppend(object o)
        {
            return (o.GetType() == typeof(string)) ? "'" + o + "'" :  o.ToString();
        }
    }
}
