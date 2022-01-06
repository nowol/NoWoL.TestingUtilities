using System;
using System.Collections;
using System.Collections.Generic;
using NoWoL.TestingUtilities.ExpectedExceptions;
using NoWoL.TestingUtilities.ObjectCreators;
using Xunit;

namespace NoWoL.TestingUtilities.Tests.ObjectCreators
{
    public class GenericDictionaryCreatorTests
    {
        private readonly GenericDictionaryCreator _sut = new();

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(IDictionary<string, string>))]
        [InlineData(typeof(Dictionary<string, string>))]
        [InlineData(typeof(Dictionary<ISomeInterface, int>))]
        [InlineData(typeof(Dictionary<ComplexTestClass, double>))]
        [InlineData(typeof(Dictionary<int, ComplexTestClass>))]
        [InlineData(typeof(Dictionary<double, ISomeInterface>))]
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
        public void UnhandledTypes(Type type)
        {
            Assert.False(_sut.CanHandle(type));
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(IDictionary<string, string>))]
        [InlineData(typeof(Dictionary<string, string>))]
        [InlineData(typeof(Dictionary<ISomeInterface, int>))]
        [InlineData(typeof(Dictionary<double, ISomeInterface>))]
        public void CreateListForType(Type type)
        {
            var result = _sut.Create(type,
                                     ParametersValidatorHelper.DefaultCreators);

            Assert.NotNull(result);
            Assert.True(result.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>));

            Assert.Single((result as IEnumerable)!);
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
            Assert.Equal("Expecting a Dictionary type however received " + type.FullName,
                         ex.Message);
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        private class TestClass
        {
            public string MethodToValidateInterface(IDictionary<int, string> paramO)
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

            public string MethodToValidateConcreteClass(Dictionary<int, string> paramO)
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

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CanHandleThrowIfInputParametersAreInvalid()
        {
            var validator = ParametersValidatorHelper.GetMethodParametersValidator(new GenericDictionaryCreator(),
                                                                                   nameof(GenericDictionaryCreator.CanHandle),
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
            var validator = ParametersValidatorHelper.GetMethodParametersValidator(new GenericDictionaryCreator(),
                                                                                   nameof(GenericDictionaryCreator.Create),
                                                                                   new object[] { typeof(Dictionary<int, int>), ParametersValidatorHelper.DefaultCreators });

            validator.SetupParameter("type",
                                     ExpectedExceptionRules.NotNull)
                     .SetupParameter("objectCreators",
                                     ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateWithExceptionForDictionary()
        {
            var obj = new TestClass();

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   nameof(TestClass.MethodToValidateConcreteClass));
            validator.SetupParameter("paramO",
                                     ExpectedExceptionRules.NotNull,
                                     ExpectedExceptionRules.NotEmpty)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateWithExceptionForIDictionary()
        {
            var obj = new TestClass();

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   nameof(TestClass.MethodToValidateInterface));
            validator.SetupParameter("paramO",
                                     ExpectedExceptionRules.NotNull,
                                     ExpectedExceptionRules.NotEmpty)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateWithoutExceptionForDictionary()
        {
            var obj = new TestClass();

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   nameof(TestClass.MethodToValidateConcreteClass),
                                                                                   new object[] { new Dictionary<int, string> { { 3, "A" } } });
            validator.SetupParameter("paramO",
                                     ExpectedExceptionRules.None)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateWithoutExceptionForIDictionary()
        {
            var obj = new TestClass();

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   nameof(TestClass.MethodToValidateInterface),
                                                                                   new object[] { new Dictionary<int, string> { { 3, "A" } } });
            validator.SetupParameter("paramO",
                                     ExpectedExceptionRules.None)
                     .Validate();
        }
    }
}