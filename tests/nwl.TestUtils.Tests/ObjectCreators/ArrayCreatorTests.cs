using System;
using System.Collections.Generic;
using nwl.TestingUtilities.ExpectedExceptions;
using nwl.TestingUtilities.ObjectCreators;
using Xunit;

namespace nwl.TestingUtilities.Tests.ObjectCreators
{
    public class ArrayCreatorTests
    {
        private readonly ArrayCreator _sut = new ArrayCreator();

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(string[]))]
        [InlineData(typeof(int[]))]
        [InlineData(typeof(double[]))]
        [InlineData(typeof(ComplexTestClass[]))]
        [InlineData(typeof(ISomeInterface[]))]
        public void HandledTypes(Type type)
        {
            Assert.True(_sut.CanHandle(type));
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(ComplexTestClass))]
        [InlineData(typeof(ISomeInterface))]
        [InlineData(typeof(IEnumerable<int>))]
        [InlineData(typeof(List<int>))]
        [InlineData(typeof(int))]
        public void UnhandledTypes(Type type)
        {
            Assert.False(_sut.CanHandle(type));
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(string[]))]
        [InlineData(typeof(int[]))]
        [InlineData(typeof(double[]))]
        [InlineData(typeof(ISomeInterface[]))]
        public void CreateArrayForType(Type type)
        {
            var result = (Array)_sut.Create(type,
                                            ArgumentsValidatorHelper.DefaultCreators);
            Assert.Single(result);
            var elementType = type.GetElementType();

            TestHelpers.AssertType(elementType,
                                   result.GetValue(0));
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
            Assert.Equal("Expecting an array type however received " + type.FullName, ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CanHandleThrowIfInputParametersAreInvalid()
        {
            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(new ArrayCreator(), nameof(ArrayCreator.CanHandle), methodArguments: new object[] { null });

            validator.SetupParameter("type", ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateThrowIfInputParametersAreInvalid()
        {
            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(new ArrayCreator(), nameof(ArrayCreator.Create), methodArguments: new object[] { typeof(int[]), ArgumentsValidatorHelper.DefaultCreators });

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

            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, nameof(TestClass.MethodToValidate), methodArguments: new object[] { new int[] { 3 } });
            validator.SetupParameter("paramO", ExpectedExceptionRules.None)
                     .Validate();
        }

        private class TestClass
        {
            public string MethodToValidate(int[] paramO)
            {
                if (paramO == null)
                {
                    throw new ArgumentNullException(nameof(paramO));
                }

                if (paramO.Length == 0)
                {
                    throw new ArgumentException("Value cannot be an empty collection.",
                                                nameof(paramO));
                }

                return null;
            }
        }
    }
}
