using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Linq.Expressions;

namespace TeamFin.Tests
{
    [TestClass]
    public class TfsPredicateVisitorSingleHardcodedClauseTests
    {
        [TestMethod]
        public void SingleHardcodedEqualsClause()
        {
            var expected = "SELECT * From WorkItems WHERE Id = 1";

            var actual = Create.Sut().Translate(a => a.Id == 1);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SingleHardcodedNotEqualsClause()
        {
            var expected = "SELECT * From WorkItems WHERE Id <> 1";

            var actual = Create.Sut().Translate(a => a.Id != 1);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SingleHardcodedLessThanClause()
        {
            var expected = "SELECT * From WorkItems WHERE Id < 1";

            var actual = Create.Sut().Translate(a => a.Id < 1);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SingleHardcodedLessThanOrEqualsClause()
        {
            var expected = "SELECT * From WorkItems WHERE Id <= 1";

            var actual = Create.Sut().Translate(a => a.Id <= 1);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SingleHardcodedGreaterThanClause()
        {
            var expected = "SELECT * From WorkItems WHERE Id > 1";

            var actual = Create.Sut().Translate(a => a.Id > 1);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SingleHardcodedGreaterThanOrEqualsClause()
        {
            var expected = "SELECT * From WorkItems WHERE Id >= 1";

            var actual = Create.Sut().Translate(a => a.Id >= 1);

            Assert.AreEqual(expected, actual);
        }
    }
  
}
