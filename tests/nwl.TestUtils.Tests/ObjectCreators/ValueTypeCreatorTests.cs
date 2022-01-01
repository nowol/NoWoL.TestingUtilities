using System;
using nwl.TestingUtilities.ExpectedExceptions;
using nwl.TestingUtilities.ObjectCreators;
using Xunit;

namespace nwl.TestingUtilities.Tests.ObjectCreators
{
    public class ValueTypeCreatorTests
    {
        private readonly ValueTypeCreator _sut = new ValueTypeCreator();

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(string))]
        [InlineData(typeof(int))]
        [InlineData(typeof(double))]
        public void HandledTypes(Type type)
        {
            Assert.True(_sut.CanHandle(type));
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(ComplexTestClass))]
        [InlineData(typeof(ISomeInterface))]
        public void UnhandledTypes(Type type)
        {
            Assert.False(_sut.CanHandle(type));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateDefaultValueForInteger()
        {
            var result = (int)_sut.Create(typeof(int),
                                          null);
            Assert.Equal(0, result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateDefaultValueForDouble()
        {
            var result = (double)_sut.Create(typeof(double),
                                          null);
            Assert.Equal(0, result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateDefaultValueForString()
        {
            var result = (string)_sut.Create(typeof(string),
                                          null);
            Assert.Equal(ValueTypeCreator.DefaultStringValue, result);
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(ComplexTestClass))]
        [InlineData(typeof(ISomeInterface))]
        public void CreateThrowsExceptionUnhandledTypes(Type type)
        {
            var ex = Assert.Throws<NotSupportedException>(() => _sut.Create(type,
                                                                            null));
            Assert.Equal("Expecting a string or value type however received " + type.FullName, ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CanHandleThrowIfInputParametersAreInvalid()
        {
            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(new ValueTypeCreator(), nameof(ValueTypeCreator.CanHandle), methodArguments: new object[] { null });

            validator.SetupParameter("type", ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateThrowIfInputParametersAreInvalid()
        {
            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(new ValueTypeCreator(), nameof(ValueTypeCreator.Create), methodArguments: new object[] { typeof(int), null });

            validator.SetupParameter("type", ExpectedExceptionRules.NotNull)
                     .SetupParameter("objectCreators", ExpectedExceptionRules.None)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateWithException()
        {
            var obj = new TestClass();

            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, nameof(TestClass.MethodToValidate));
            validator.SetupParameter("paramO", ExpectedExceptionRules.NotValue(3))
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateWithoutException()
        {
            var obj = new TestClass();

            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, nameof(TestClass.MethodToValidate));
            validator.SetupParameter("paramO", ExpectedExceptionRules.None)
                     .Validate();
        }

        private class TestClass
        {
            public string MethodToValidate(int paramO)
            {
                if (paramO == 3)
                {
                    throw new ArgumentNullException(nameof(paramO));
                }

                return null;
            }
        }
    }
}
