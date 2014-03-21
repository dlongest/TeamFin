using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TeamFin.Tests
{
    [TestClass]
    public class TfsPredicateVisitorStringMethodTests
    {
        [TestMethod]
        public void Contains_IsCorrectlyConverted()
        {
            var expected = "SELECT * From WorkItems WHERE Title contains 'text'";

            var actual = Create.Sut().Translate(a => a.Title.Contains("text"));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void StartsWith_IsCorrectlyConverted()
        {
            var expected = "SELECT * From WorkItems WHERE Title contains 'text'";

            var actual = Create.Sut().Translate(a => a.Title.StartsWith("text"));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EndsWith_IsCorrectlyConverted()
        {
            var expected = "SELECT * From WorkItems WHERE Title contains 'text'";

            var actual = Create.Sut().Translate(a => a.Title.EndsWith("text"));
            
            Assert.AreEqual(expected, actual);
        }
                

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Reverse_ThrowsException()
        { 
            Create.Sut().Translate(a => a.Title.Reverse() == "text");
        }
    }
}
