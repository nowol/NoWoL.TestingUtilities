using System;
using NoWoL.TestingUtilities.ExpectedExceptions;
using Xunit;

namespace NoWoL.TestingUtilities.Tests.ExpectedExceptions
{
    public class ExpectedNoExceptionTests : ExpectedExceptionBaseTests<ExpectedNoExceptionRule>
    {
        protected override object GetInvalidParameterValueExpectedValue()
        {
            return GetInvalidParameterValueDefaultValue();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ReturnsExpectedInvalidValue()
        {
            Assert.Null(_sut.GetInvalidParameterValue(MethodsHolder.GetStringParameterInfo(), null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetInvalidParameterValueThrowIfInputParametersAreInvalid()
        {
            var validator = ParametersValidatorHelper.GetMethodParametersValidator(new ExpectedNoExceptionRule(), nameof(ExpectedNoExceptionRule.GetInvalidParameterValue), methodParameters: new object[] { MethodsHolder.GetStringParameterInfo(), null });

            validator.SetupParameter("parameterInfo", ExpectedExceptionRules.NotNull)
                     .SetupParameter("defaultValue", ExpectedExceptionRules.None)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EvaluateReturnsFalseIfExceptionIsNotNull()
        {
            var result = _sut.Evaluate("paramName",
                                       new ArgumentNullException("paramName"),
                                       out var additionalMessage);
            Assert.False(result);
            Assert.StartsWith(ExpectedNoExceptionRule.MissingException,
                              additionalMessage,
                              StringComparison.Ordinal);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EvaluateReturnsTrueIfExceptionIsNull()
        {
            var result = _sut.Evaluate("paramName",
                                       null,
                                       out var additionalMessage);
            Assert.True(result);
            Assert.Null(additionalMessage);
        }
    }
}
