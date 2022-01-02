using System;
using NoWoL.TestingUtilities.ExpectedExceptions;
using Xunit;

namespace NoWoL.TestingUtilities.Tests.ExpectedExceptions
{
    public class ExpectedExceptionWithInvalidIntegerValueTests : ExpectedExceptionWithInvalidValueBaseTests<int>
    {
        public ExpectedExceptionWithInvalidIntegerValueTests()
            : base(10, 20)
        { }
    }

    public class ExpectedExceptionWithInvalidDoubleValueTests : ExpectedExceptionWithInvalidValueBaseTests<double>
    {
        public ExpectedExceptionWithInvalidDoubleValueTests()
            : base(10, 10.1)
        { }
    }

    public class ExpectedExceptionWithInvalidStringValueTests : ExpectedExceptionWithInvalidValueBaseTests<string>
    {
        public ExpectedExceptionWithInvalidStringValueTests()
            : base("Freddie", "Mercury")
        { }
    }

    public abstract class ExpectedExceptionWithInvalidValueBaseTests<T>
    {
        private class InvalidValueTestClass
        {
            private readonly T _invalidValue;

            public InvalidValueTestClass(T invalidValue)
            {
                _invalidValue = invalidValue;
            }

            public string MethodToValidate(T paramO)
            {
                if (Object.Equals(paramO,_invalidValue))
                {
                    throw new ArgumentException("Value cannot be " + _invalidValue, nameof(paramO));
                }

                return null;
            }
        }

        protected readonly ExpectedExceptionRuleWithInvalidValue<T> _sut;
        private readonly T _invalidValue;
        private readonly T _validValue;

        protected ExpectedExceptionWithInvalidValueBaseTests(T invalidValue, T validValue)
        {
            _invalidValue = invalidValue;
            _validValue = validValue;
            _sut = new ExpectedExceptionRuleWithInvalidValue<T>(invalidValue);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ReturnsName()
        {
            Assert.Equal("ExpectedExceptionRuleWithInvalidValue for type " + typeof(T).Name,
                         _sut.Name);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void EvaluateThrowsIfParamNameIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _sut.Evaluate(null, null, out _));
            Assert.Equal("paramName",
                         ex.ParamName);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ReturnsExpectedInvalidValue()
        {
            Assert.Equal(_invalidValue,
                         _sut.GetInvalidParameterValue(MethodsHolder.GetStringParameterInfo(), null));
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetInvalidParameterValueThrowIfInputParametersAreInvalid()
        {
            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(new ExpectedExceptionRuleWithInvalidValue<T>(_invalidValue), nameof(ExpectedExceptionRuleWithInvalidValue<T>.GetInvalidParameterValue), methodArguments: new object[] { MethodsHolder.GetStringParameterInfo(), null });

            validator.SetupParameter("parameterInfo", ExpectedExceptionRules.NotNull)
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
            Assert.Equal(ExpectedExceptionRuleBase.NoExceptionMessage,
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
            Assert.Equal($"An ArgumentException for the parameter 'paramName' was expected however the exception is for parameter 'NotParamName'",
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

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorWithInvalidValue()
        {
            var obj = new InvalidValueTestClass(_invalidValue);
            
            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, nameof(InvalidValueTestClass.MethodToValidate));
            validator.SetupParameter("paramO", ExpectedExceptionRules.NotValue(_invalidValue))
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorWithValidValue()
        {
            var obj = new InvalidValueTestClass(_validValue);
            
            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, nameof(InvalidValueTestClass.MethodToValidate));
            validator.SetupParameter("paramO", ExpectedExceptionRules.None)
                     .Validate();
        }
    }
}
