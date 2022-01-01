using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using nwl.TestingUtilities.ExpectedExceptions;
using nwl.TestingUtilities.ObjectCreators;
using Xunit;

namespace nwl.TestingUtilities.Tests.ObjectCreators
{
    public class ListCreatorTests
    {
        private readonly ListCreator _sut = new ListCreator();

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(List<int>))]
        [InlineData(typeof(List<double>))]
        [InlineData(typeof(List<ComplexTestClass>))]
        [InlineData(typeof(List<ISomeInterface>))]
        public void HandledTypes(Type type)
        {
            Assert.True(_sut.CanHandle(type));
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(ComplexTestClass))]
        [InlineData(typeof(ISomeInterface))]
        [InlineData(typeof(int))]
        [InlineData(typeof(int[]))]
        [InlineData(typeof(IEnumerable<int>))]
        public void UnhandledTypes(Type type)
        {
            Assert.False(_sut.CanHandle(type));
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(List<int>))]
        [InlineData(typeof(List<double>))]
        [InlineData(typeof(List<ISomeInterface>))]
        public void CreateArrayForType(Type type)
        {
            var result = (IList)_sut.Create(type,
                                            ArgumentsValidatorHelper.DefaultCreators);
            Assert.Single(result);
            var elementType = type.GenericTypeArguments.Single();

            TestHelpers.AssertType(elementType,
                                   result[0]);
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(ComplexTestClass))]
        [InlineData(typeof(ISomeInterface))]
        public void CreateThrowsExceptionUnhandledTypes(Type type)
        {
            var ex = Assert.Throws<NotSupportedException>(() => _sut.Create(type,
                                                                            ArgumentsValidatorHelper.DefaultCreators));
            Assert.Equal("Expecting a List<> type however received " + type.FullName, ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CanHandleThrowIfInputParametersAreInvalid()
        {
            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(new ListCreator(), nameof(ListCreator.CanHandle), methodArguments: new object[] { null });

            validator.SetupParameter("type", ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateThrowIfInputParametersAreInvalid()
        {
            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(new ListCreator(), nameof(ListCreator.Create), methodArguments: new object[] { typeof(List<int>), ArgumentsValidatorHelper.DefaultCreators });

            validator.SetupParameter("type", ExpectedExceptionRules.NotNull)
                     .SetupParameter("objectCreators", ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateWithException()
        {
            var obj = new TestClass();

            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, nameof(TestClass.MethodToValidate));
            validator.SetupParameter("paramO", ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmpty)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateWithoutException()
        {
            var obj = new TestClass();

            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, nameof(TestClass.MethodToValidate), methodArguments: new object[] { new List<int> { 3 } });
            validator.SetupParameter("paramO", ExpectedExceptionRules.None)
                     .Validate();
        }

        private class TestClass
        {
            public string MethodToValidate(List<int> paramO)
            {
                if (paramO == null)
                {
                    throw new ArgumentNullException(nameof(paramO));
                }

                if (paramO.Count == 0)
                {
                    throw new ArgumentException("Value cannot be an empty collection.",
                                                nameof(paramO));
                }

                return null;
            }
        }
    }
}
