using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TeamFin.Tests
{
    [TestClass]
    public class TfsPredicateVisitorEnumPropertyTests
    {
        [TestMethod]
        public void EnumsAreConvertedToStringRepresentation()
        {

            var expected = "SELECT * From WorkItems WHERE Test = 'One'";

            var actual = new TfsPredicateVisitor<EnumTest>(FieldMappings.Map<EnumTest>()).Translate(a => a.Test == SampleEnum.One);

            Assert.AreEqual(expected, actual);

        }
    }

    public class EnumTest
    {
        public int Id { get; set; }

        public string Property { get; set; }

        public SampleEnum Test { get; set; }
    }

    public enum SampleEnum
    {
        One, Two, Three
    }
}
