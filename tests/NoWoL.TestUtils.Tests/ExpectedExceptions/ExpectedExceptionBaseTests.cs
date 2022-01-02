using System;
using Xunit;

namespace NoWoL.TestingUtilities.Tests.ExpectedExceptions
{
    public abstract class ExpectedExceptionBaseTests<T>
        where T: IExpectedExceptionRule, new()
    {
        protected readonly T _sut = new();

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ReturnsName()
        {
            Assert.Equal(typeof(T).Name,
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
        public void GetInvalidParameterValueReturnsTheExpectedValue()
        {
            var result = _sut.GetInvalidParameterValue(MethodsHolder.GetStringParameterInfo(),
                                                       GetInvalidParameterValueDefaultValue());
            Assert.Equal(GetInvalidParameterValueExpectedValue(),
                         result);
        }

        protected virtual object GetInvalidParameterValueDefaultValue() => 23;
        protected virtual object GetInvalidParameterValueExpectedValue() => null;
    }
}