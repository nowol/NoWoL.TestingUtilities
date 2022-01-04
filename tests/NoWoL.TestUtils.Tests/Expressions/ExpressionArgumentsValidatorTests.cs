using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoWoL.TestingUtilities.ExpectedExceptions;
using NoWoL.TestingUtilities.Expressions;
using Xunit;

namespace NoWoL.TestingUtilities.Tests.Expressions
{
    public class ExpressionArgumentsValidatorTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateWithInvalidArguments()
        {
            var validator = ArgumentsValidatorHelper.GetConstructorArgumentsValidator<ExpressionArgumentsValidator<TestClass>>(new object[] { new TestClass(), Array.Empty<IObjectCreator>() });
            validator.SetupParameter("targetObject", ExpectedExceptionRules.NotNull)
                     .SetupParameter("objectCreators", ExpectedExceptionRules.None)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ForWithInvalidArguments()
        {
            var method = typeof(ExpressionArgumentsValidator<TestClass>).GetMethod(nameof(ExpressionArgumentsValidator<TestClass>.For));
            var genericMethod = method!.MakeGenericMethod(typeof(int));
            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(new ExpressionArgumentsValidator<TestClass>(new TestClass(), null),
                                                                                 genericMethod);

            validator.SetupParameter("rules", 
                                     ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmpty)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetupCreatesAValidValidator()
        {
            var sut = new ExpressionArgumentsValidator<TestClass>(new TestClass(), null);
            sut.Setup(x => x.SomeMethod(sut.For<int>(ExpectedExceptionRules.NotValue(0)),
                                        sut.For<string>(ExpectedExceptionRules.NotNull)))
               .Validate();
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(int), 0)]
        [InlineData(typeof(double), 0d)]
        [InlineData(typeof(int?), null)]
        [InlineData(typeof(TestClass), null)]
        [InlineData(typeof(string), null)]
        public void ForReturnTheDefaultOfTheType(Type type, object expectedValue)
        {
            var sut = new ExpressionArgumentsValidator<TestClass>(new TestClass(), null);

            var method = typeof(ExpressionArgumentsValidator<TestClass>).GetMethod(nameof(ExpressionArgumentsValidator<TestClass>.For));
            var genericMethod = method!.MakeGenericMethod(type);

            var result = genericMethod.Invoke(sut,
                                              new object[] { new [] { ExpectedExceptionRules.NotNull }});
            Assert.Equal(expectedValue,
                         result);
        }

        [ExcludeFromCodeCoverage]
        private class TestClass
        {
            public int SomeMethod(int i, string s)
            {
                if (s == null)
                {
                    throw new ArgumentNullException(nameof(s));
                }

                if (i == 0)
                {
                    throw new ArgumentException("Cannot be zero",
                                                nameof(i));
                }

                return default;
            }
        }
    }
}
