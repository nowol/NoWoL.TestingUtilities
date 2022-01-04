using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoWoL.TestingUtilities.Exceptions;
using Xunit;

namespace NoWoL.TestingUtilities.Tests.Exceptions
{
    public class ExceptionFormattersTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void TypeFullNameFormatterReturnsEmptyStringForEmptyType()
        {
            Assert.Equal(String.Empty,
                         ExceptionFormatters.TypeFullNameFormatter(null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void TypeFullNameFormatterReturnsFullNameOfTheType()
        {
            Assert.Equal("NoWoL.TestingUtilities.Tests.Exceptions.ExceptionFormattersTests",
                         ExceptionFormatters.TypeFullNameFormatter(typeof(ExceptionFormattersTests)));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void JoinCommaFormatterReturnsEmptyStringForNullInput()
        {
            Assert.Equal(String.Empty,
                         ExceptionFormatters.JoinCommaFormatter(null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void JoinCommaFormatterReturnsCsv()
        {
            Assert.Equal("a, b, c",
                         ExceptionFormatters.JoinCommaFormatter(new []{ "a", "b", "c"}));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AddSpaceWhenRequiredFormatterReturnsEmptyStringForNullInput()
        {
            Assert.Equal(String.Empty,
                         ExceptionFormatters.AddSpaceWhenRequiredFormatter(null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AddSpaceWhenRequiredFormatterReturnsEmptyStringForEmptyInput()
        {
            Assert.Equal(String.Empty,
                         ExceptionFormatters.AddSpaceWhenRequiredFormatter(String.Empty));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AddSpaceWhenRequiredFormatterReturnsEmptyStringForWhiteSpaceInput()
        {
            Assert.Equal(String.Empty,
                         ExceptionFormatters.AddSpaceWhenRequiredFormatter("       "));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AddSpaceWhenRequiredFormatterAddsSpaceBeforeValue()
        {
            Assert.Equal(" value",
                         ExceptionFormatters.AddSpaceWhenRequiredFormatter("value"));
        }
    }
}
