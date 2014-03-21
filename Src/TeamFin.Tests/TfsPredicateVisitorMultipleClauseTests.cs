using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TeamFin.Tests
{
    [TestClass]
    public class TfsPredicateVisitorMultipleClauseTests
    {
        [TestMethod]
        public void AndWithTwoHardcodedValues()
        {
            var expected = "SELECT * From WorkItems WHERE (Id = 1 AND Project = 'Project')";

            var actual = Create.Sut().Translate(a => a.Id == 1 && a.Project == "Project");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OrWithTwoHardcodedValues()
        {
            var expected = "SELECT * From WorkItems WHERE (Id = 1 OR Project = 'Project')";

            var actual = Create.Sut().Translate(a => a.Id == 1 || a.Project == "Project");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AndWithOneHardcodedAndOneVariableValues()
        {
            int id = 1;
            var expected = "SELECT * From WorkItems WHERE (Id = 1 AND Project = 'Project')";

            var actual = Create.Sut().Translate(a => a.Id == id && a.Project == "Project");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OrWithOneHardcodedAndOneVariableValues()
        {
            int id = 1;
            var expected = "SELECT * From WorkItems WHERE (Id = 1 OR Project = 'Project')";

            var actual = Create.Sut().Translate(a => a.Id == id || a.Project == "Project");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AndWithTwoVariableValues()
        {
            int id = 1;
            string project = "Project";

            var expected = "SELECT * From WorkItems WHERE (Id = 1 AND Project = 'Project')";

            var actual = Create.Sut().Translate(a => a.Id == id && a.Project == project);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OrWithTwoVariableValues()
        {
            int id = 1;
            string project = "Project";

            var expected = "SELECT * From WorkItems WHERE (Id = 1 OR Project = 'Project')";

            var actual = Create.Sut().Translate(a => a.Id == id || a.Project == project);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TwoAndConditions()
        {
            int id = 1;
            string project = "Project";
            string iterationPath = "This Path";

            var expected = "SELECT * From WorkItems WHERE ((Id = 1 AND Project = 'Project') AND IterationPath = 'This Path')";

            var actual = Create.Sut().Translate(a => a.Id == id && a.Project == project && a.IterationPath == iterationPath);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TwoOrConditions()
        {
            int id = 1;
            string project = "Project";
            string iterationPath = "This Path";

            var expected = "SELECT * From WorkItems WHERE ((Id = 1 OR Project = 'Project') OR IterationPath = 'This Path')";

            var actual = Create.Sut().Translate(a => a.Id == id || a.Project == project || a.IterationPath == iterationPath);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OneAndWithOneOr()
        {
            int id = 1;
            string project = "Project";
            string iterationPath = "This Path";

            var expected = "SELECT * From WorkItems WHERE ((Id = 1 AND Project = 'Project') OR IterationPath = 'This Path')";

            var actual = Create.Sut().Translate(a => (a.Id == id && a.Project == project || a.IterationPath == iterationPath));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OneOrdWithOneAnd()
        {
            int id = 1;
            string project = "Project";
            string iterationPath = "This Path";

            var expected = "SELECT * From WorkItems WHERE (Id = 1 AND (Project = 'Project' OR IterationPath = 'This Path'))";

            var actual = Create.Sut().Translate(a => a.Id == id && (a.Project == project || a.IterationPath == iterationPath));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AndsHavePrecedenceOverOr()
        {
            int id = 1;
            string project = "Project";
            string iterationPath = "This Path";

            var expected = "SELECT * From WorkItems WHERE (Id = 1 OR (Project = 'Project' AND IterationPath = 'This Path'))";

            var actual = Create.Sut().Translate(a => a.Id == id || a.Project == project && a.IterationPath == iterationPath);

            Assert.AreEqual(expected, actual);
        }

    }
}
