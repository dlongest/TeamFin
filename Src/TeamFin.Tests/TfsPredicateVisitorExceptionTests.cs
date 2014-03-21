using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TeamFin.Tests
{
    [TestClass]
    public class TfsPredicateVisitorExceptionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsException_IfExpression_IsNull()
        {
            Create.Sut().Translate(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ThrowsException_IfExpression_IsNotExpressionOfFuncTBool()
        {
            Expression<Func<TfsWorkItem, string>> f = a => a.Project;

            Create.Sut().Translate(f);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsException_IfFieldMappings_IsNull()
        {
            new TfsPredicateVisitor<TfsWorkItem>(null);
        }
    }


    
}
