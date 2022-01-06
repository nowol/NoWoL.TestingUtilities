using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NoWoL.TestingUtilities.ExpectedExceptions;
using NoWoL.TestingUtilities.ObjectCreators;
using Xunit;

namespace NoWoL.TestingUtilities.Tests.ObjectCreators
{
    public class GenericListCreatorTests
    {
        private readonly GenericListCreator _sut = new();

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(IList<string>))]
        [InlineData(typeof(IList<int>))]
        [InlineData(typeof(IList<double>))]
        [InlineData(typeof(List<double>))]
        [InlineData(typeof(IList<ComplexTestClass>))]
        [InlineData(typeof(IList<ISomeInterface>))]
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
        [InlineData(typeof(IList))]
        public void UnhandledTypes(Type type)
        {
            Assert.False(_sut.CanHandle(type));
        }

        [Theory]
        [Trait("Category",
               "Unit")]
        [InlineData(typeof(IList<string>))]
        [InlineData(typeof(IList<int>))]
        [InlineData(typeof(IList<double>))]
        [InlineData(typeof(List<double>))]
        [InlineData(typeof(IList<ISomeInterface>))]
        public void CreateListForType(Type type)
        {
            var result = (IList)_sut.Create(type,
                                            ParametersValidatorHelper.DefaultCreators);
            Assert.Single(result);
#pragma warning disable CA1062 // Validate arguments of public methods
            var elementType = type.GenericTypeArguments.Single();
#pragma warning restore CA1062 // Validate arguments of public methods

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
            var ex = Assert.Throws<UnsupportedTypeException>(() => _sut.Create(type,
                                                                               ParametersValidatorHelper.DefaultCreators));
#pragma warning disable CA1062 // Validate arguments of public methods
            Assert.Equal("Expecting an IList<> type however received " + type.FullName,
                         ex.Message);
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        private class TestClass
        {
            public string MethodToValidateForIList(IList<int> paramO)
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

            public string MethodToValidateForList(List<int> paramO)
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
            var validator = ParametersValidatorHelper.GetMethodParametersValidator(new GenericListCreator(),
                                                                                   nameof(GenericListCreator.CanHandle),
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
            var validator = ParametersValidatorHelper.GetMethodParametersValidator(new GenericListCreator(),
                                                                                   nameof(GenericListCreator.Create),
                                                                                   new object[] { typeof(IList<int>), ParametersValidatorHelper.DefaultCreators });

            validator.SetupParameter("type",
                                     ExpectedExceptionRules.NotNull)
                     .SetupParameter("objectCreators",
                                     ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateWithExceptionForIList()
        {
            var obj = new TestClass();

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   nameof(TestClass.MethodToValidateForIList));
            validator.SetupParameter("paramO",
                                     ExpectedExceptionRules.NotNull,
                                     ExpectedExceptionRules.NotEmpty)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateWithExceptionForList()
        {
            var obj = new TestClass();

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   nameof(TestClass.MethodToValidateForList));
            validator.SetupParameter("paramO",
                                     ExpectedExceptionRules.NotNull,
                                     ExpectedExceptionRules.NotEmpty)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateWithoutExceptionForIList()
        {
            var obj = new TestClass();

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   nameof(TestClass.MethodToValidateForIList),
                                                                                   new object[] { new List<int> { 3 } });
            validator.SetupParameter("paramO",
                                     ExpectedExceptionRules.None)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateWithoutExceptionForList()
        {
            var obj = new TestClass();

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   nameof(TestClass.MethodToValidateForList),
                                                                                   new object[] { new List<int> { 3 } });
            validator.SetupParameter("paramO",
                                     ExpectedExceptionRules.None)
                     .Validate();
        }
    }
}