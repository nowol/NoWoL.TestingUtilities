using System;
using Moq;
using nwl.TestingUtilities.ExpectedExceptions;
using nwl.TestingUtilities.ObjectCreators;
using Xunit;

namespace nwl.TestingUtilities.Tests.ObjectCreators
{
    public class MoqInterfaceCreatorTests
    {
        private readonly MoqInterfaceCreator _sut = new MoqInterfaceCreator();

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CanHandleInterfaces()
        {
            Assert.True(_sut.CanHandle(typeof(ISomeInterface)));
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(ComplexTestClass))]
        [InlineData(typeof(int))]
        [InlineData(typeof(double))]
        public void UnhandledTypes(Type type)
        {
            Assert.False(_sut.CanHandle(type));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateMockOfInterface()
        {
            var result = _sut.Create(typeof(ISomeInterface),
                                     null) as ISomeInterface;
            var mock = Mock.Get(result);
            Assert.NotNull(mock);
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(ComplexTestClass))]
        [InlineData(typeof(int))]
        [InlineData(typeof(double))]
        public void CreateThrowsExceptionUnhandledTypes(Type type)
        {
            var ex = Assert.Throws<NotSupportedException>(() => _sut.Create(type,
                                                                            null));
            Assert.Equal("Expecting an interface however received " + type.FullName, ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CanHandleThrowIfInputParametersAreInvalid()
        {
            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(new MoqInterfaceCreator(), nameof(MoqInterfaceCreator.CanHandle), methodArguments: new object[] { null });

            validator.SetupParameter("type", ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateThrowIfInputParametersAreInvalid()
        {
            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(new MoqInterfaceCreator(), nameof(MoqInterfaceCreator.Create), methodArguments: new object[] { typeof(ISomeInterface), null });

            validator.SetupParameter("type", ExpectedExceptionRules.NotNull)
                     .SetupParameter("objectCreators", ExpectedExceptionRules.None)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateWithException()
        {
            var obj = new TestClass(null);

            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, nameof(TestClass.MethodToValidate));
            validator.SetupParameter("paramO", ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateWithoutException()
        {
            var obj = new TestClass(new Mock<ISomeInterface>().Object);

            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, nameof(TestClass.MethodToValidate));
            validator.SetupParameter("paramO", ExpectedExceptionRules.None)
                     .Validate();
        }

        private class TestClass
        {
            private readonly ISomeInterface _expected;
            public TestClass(ISomeInterface expected)
            {
                _expected = expected;
            }

            public string MethodToValidate(ISomeInterface paramO)
            {
                if (paramO == _expected)
                {
                    throw new ArgumentNullException(nameof(paramO));
                }

                return null;
            }
        }
    }
}
