using System;
using System.Collections.Generic;
using NoWoL.TestingUtilities.ExpectedExceptions;
using Xunit;

namespace NoWoL.TestingUtilities.Tests.ExpectedExceptions
{
    public class SkipParameterExceptionRuleTests
    {
        protected readonly SkipParameterExceptionRule _sut = new();

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ReturnsName()
        {
            Assert.Equal(nameof(SkipParameterExceptionRule),
                         _sut.Name);
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
            var validator = ParametersValidatorHelper.GetMethodParametersValidator(new SkipParameterExceptionRule(), nameof(SkipParameterExceptionRule.GetInvalidParameterValue), methodParameters: new object[] { MethodsHolder.GetStringParameterInfo(), null });

            validator.SetupParameter("parameterInfo", ExpectedExceptionRules.NotNull)
                     .SetupParameter("defaultValue", ExpectedExceptionRules.None)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EvaluateReturnsAlwaysReturnFalse()
        {
            var result = _sut.Evaluate("paramName",
                                       new ArgumentNullException("paramName"),
                                       out var additionalMessage);
            Assert.False(result);
            Assert.Null(additionalMessage);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsSkipParameterExceptionRuleReturnsFalseIfRuleIsNullOneItem()
        {
            Assert.False(SkipParameterExceptionRule.IsSkipParameterExceptionRule((IExpectedExceptionRule)null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsSkipParameterExceptionRuleReturnsFalseIfRuleIsNotExpectedTypeOneItem()
        {
            Assert.False(SkipParameterExceptionRule.IsSkipParameterExceptionRule(new ExpectedNoExceptionRule()));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsSkipParameterExceptionRuleReturnsTrueForCorrectTypeOneItem()
        {
            Assert.True(SkipParameterExceptionRule.IsSkipParameterExceptionRule(new SkipParameterExceptionRule()));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsSkipParameterExceptionRuleReturnsFalseIfRuleIsNullList()
        {
            Assert.False(SkipParameterExceptionRule.IsSkipParameterExceptionRule((IEnumerable<IExpectedExceptionRule>)null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsSkipParameterExceptionRuleReturnsFalseIfRuleIsNotExpectedTypeList()
        {
            Assert.False(SkipParameterExceptionRule.IsSkipParameterExceptionRule(new [] {new ExpectedNoExceptionRule()}));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void IsSkipParameterExceptionRuleReturnsTrueForCorrectTypeList()
        {
            Assert.True(SkipParameterExceptionRule.IsSkipParameterExceptionRule(new[] { new SkipParameterExceptionRule()}));
        }
    }
}
