﻿using System;
using NoWoL.TestingUtilities.ExpectedExceptions;
using Xunit;

namespace NoWoL.TestingUtilities.Tests.ExpectedExceptions
{
    public class ExpectedNotEmptyExceptionTests : ExpectedExceptionBaseTests<ExpectedNotEmptyException>
    {
        protected override object GetInvalidParameterValueExpectedValue()
        {
            return String.Empty;
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ReturnsExpectedInvalidValueForString()
        {
            Assert.Equal(String.Empty, _sut.GetInvalidParameterValue(MethodsHolder.GetStringParameterInfo(), null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ReturnsExpectedInvalidValueForArray()
        {
            Assert.Equal(String.Empty, _sut.GetInvalidParameterValue(MethodsHolder.GetStringArrayParameterInfo(), null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ReturnsExpectedInvalidValueForList()
        {
            Assert.Equal(String.Empty, _sut.GetInvalidParameterValue(MethodsHolder.GetStringListParameterInfo(), null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ReturnsExpectedInvalidValueForIEnumerable()
        {
            Assert.Equal(String.Empty, _sut.GetInvalidParameterValue(MethodsHolder.GetStringIEnumerableParameterInfo(), null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ReturnsExpectedInvalidValueForICollection()
        {
            Assert.Equal(String.Empty, _sut.GetInvalidParameterValue(MethodsHolder.GetStringICollectionParameterInfo(), null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ReturnsExpectedInvalidValueForIList()
        {
            Assert.Equal(String.Empty, _sut.GetInvalidParameterValue(MethodsHolder.GetStringIListParameterInfo(), null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetInvalidParameterValueForUnknownType()
        {
            var ex = Assert.Throws<NotSupportedException>(() => _sut.GetInvalidParameterValue(MethodsHolder.GetIntParameterInfo(), null));
            Assert.Equal("Unknown type: System.Int32",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetInvalidParameterValueForUnknownGenericType()
        {
            var ex = Assert.Throws<NotSupportedException>(() => _sut.GetInvalidParameterValue(MethodsHolder.GetActionParameterInfo(), null));
            Assert.StartsWith("Unknown type: System.Action",
                              ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetInvalidParameterValueThrowIfInputParametersAreInvalid()
        {
            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(new ExpectedNotEmptyException(), nameof(ExpectedNotEmptyException.GetInvalidParameterValue), methodArguments: new object[] { MethodsHolder.GetStringParameterInfo(), null });

            validator.SetupParameter("param", ExpectedExceptionRules.NotNull)
                     .SetupParameter("defaultValue", ExpectedExceptionRules.None)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EvaluateReturnsFalseIfExceptionIsNull()
        {
            var result = _sut.Evaluate("paramName",
                                       null,
                                       out var additionalMessage);
            Assert.False(result);
            Assert.Equal(ExpectedExceptionBase.NoExceptionMessage,
                         additionalMessage);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EvaluateReturnsFalseIfExceptionIsUnknownType()
        {
            var result = _sut.Evaluate("paramName",
                                       new NotSupportedException(),
                                       out var additionalMessage);
            Assert.False(result);
            Assert.Null(additionalMessage);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EvaluateReturnsFalseIfParamNameIsNotTheExpectedOne()
        {
            var result = _sut.Evaluate("paramName",
                                       new ArgumentNullException("NotParamName"),
                                       out var additionalMessage);
            Assert.False(result);
            Assert.Equal("An ArgumentException for the parameter 'paramName' was expected however the exception is for parameter 'NotParamName'",
                         additionalMessage);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EvaluateReturnsTrueForExpectedException()
        {
            var result = _sut.Evaluate("paramName",
                                       new ArgumentNullException("paramName"),
                                       out var additionalMessage);
            Assert.True(result);
            Assert.Null(additionalMessage);
        }
    }
}