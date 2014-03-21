using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TeamFin.Tests
{
    [TestClass]
    public class TfsPredicateVisitorSingleVariableClauseTests
    {
        [TestMethod]
        public void SingleVariableEqualsClause()
        {
            int id = 1;
            var expected = "SELECT * From WorkItems WHERE Id = 1";

            var actual = Create.Sut().Translate(a => a.Id == id);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SingleVariableNotEqualsClause()
        {
            int id = 1;
            var expected = "SELECT * From WorkItems WHERE Id <> 1";

            var actual = Create.Sut().Translate(a => a.Id != id);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SingleVariableLessThanClause()
        {
            int id = 1;
            var expected = "SELECT * From WorkItems WHERE Id < 1";

            var actual = Create.Sut().Translate(a => a.Id < id);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SingleVariableLessThanOrEqualsClause()
        {
            int id = 1;
            var expected = "SELECT * From WorkItems WHERE Id <= 1";

            var actual = Create.Sut().Translate(a => a.Id <= id);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SingleVariableGreaterThanClause()
        {
            int id = 1;
            var expected = "SELECT * From WorkItems WHERE Id > 1";

            var actual = Create.Sut().Translate(a => a.Id > id);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SingleVariableGreaterThanOrEqualsClause()
        {
            int id = 1;
            var expected = "SELECT * From WorkItems WHERE Id >= 1";

            var actual = Create.Sut().Translate(a => a.Id >= id);

            Assert.AreEqual(expected, actual);
        }
    }
}