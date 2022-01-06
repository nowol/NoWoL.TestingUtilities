using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NoWoL.TestingUtilities.ExpectedExceptions;
using NoWoL.TestingUtilities.ObjectCreators;
using Xunit;

namespace NoWoL.TestingUtilities.Tests.ObjectCreators
{
    public class GenericIEnumerableCreatorTests
    {
        private readonly GenericIEnumerableCreator _sut = new();

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(IEnumerable<string>))]
        [InlineData(typeof(IEnumerable<int>))]
        [InlineData(typeof(IEnumerable<double>))]
        [InlineData(typeof(IEnumerable<ComplexTestClass>))]
        [InlineData(typeof(IEnumerable<ISomeInterface>))]
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
        [InlineData(typeof(IEnumerable))]
        public void UnhandledTypes(Type type)
        {
            Assert.False(_sut.CanHandle(type));
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(IEnumerable<string>))]
        [InlineData(typeof(IEnumerable<int>))]
        [InlineData(typeof(IEnumerable<double>))]
        [InlineData(typeof(IEnumerable<ISomeInterface>))]
        public void CreateArrayForType(Type type)
        {
            var result = (Array)_sut.Create(type,
                                            ParametersValidatorHelper.DefaultCreators);
            Assert.Single(result);
#pragma warning disable CA1062 // Validate arguments of public methods
            var elementType = type.GenericTypeArguments.Single();
#pragma warning restore CA1062 // Validate arguments of public methods

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
            var ex = Assert.Throws<UnsupportedTypeException>(() => _sut.Create(type,
                                                                               ParametersValidatorHelper.DefaultCreators));
#pragma warning disable CA1062 // Validate arguments of public methods
            Assert.Equal("Expecting an IEnumerable<> type however received " + type.FullName,
                         ex.Message);
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        private class TestClass
        {
            public string MethodToValidate(IEnumerable<int> paramO)
            {
                if (paramO == null)
                {
                    throw new ArgumentNullException(nameof(paramO));
                }

                if (!paramO.Any())
                {
                    throw new ArgumentException("Value cannot be an empty collection.",
                                                nameof(paramO));
                }

                return null;
            }
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CanHandleThrowIfInputParametersAreInvalid()
        {
            var validator = ParametersValidatorHelper.GetMethodParametersValidator(new GenericIEnumerableCreator(),
                                                                                   nameof(GenericIEnumerableCreator.CanHandle),
                                                                                   new object[] { null });

            validator.SetupParameter("type",
                                     ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateThrowIfInputParametersAreInvalid()
        {
            var validator = ParametersValidatorHelper.GetMethodParametersValidator(new GenericIEnumerableCreator(),
                                                                                   nameof(GenericIEnumerableCreator.Create),
                                                                                   new object[] { typeof(IEnumerable<int>), ParametersValidatorHelper.DefaultCreators });

            validator.SetupParameter("type",
                                     ExpectedExceptionRules.NotNull)
                     .SetupParameter("objectCreators",
                                     ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateWithException()
        {
            var obj = new TestClass();

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   nameof(TestClass.MethodToValidate));
            validator.SetupParameter("paramO",
                                     ExpectedExceptionRules.NotNull,
                                     ExpectedExceptionRules.NotEmpty)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateWithoutException()
        {
            var obj = new TestClass();

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   nameof(TestClass.MethodToValidate),
                                                                                   new object[] { new List<int> { 3 } });
            validator.SetupParameter("paramO",
                                     ExpectedExceptionRules.None)
                     .Validate();
        }
    }
}