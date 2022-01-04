using System.Linq;
using NoWoL.TestingUtilities.Expressions;
using Xunit;

namespace NoWoL.TestingUtilities.Tests.Expressions
{
    public class ExpressionMethodInfoTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void SettingTheMethodNameAllowsUsToReadItBack()
        {
            var sut = new ExpressionMethodInfo();
            sut.MethodName = "Freddie";
            Assert.Equal("Freddie",
                         sut.MethodName);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SettingTheMethodAllowsUsToReadItBack()
        {
            var sut = new ExpressionMethodInfo();
            var methodInfo = typeof(ExpressionMethodInfoTests).GetMethods().First();
            sut.Method = methodInfo;
            Assert.Same(methodInfo,
                        sut.Method);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SettingTheArgumentValuesAllowsUsToReadItBack()
        {
            var sut = new ExpressionMethodInfo();
            sut.ArgumentValues.Add(new ExpressionArgumentInfo
                                   {
                                       Name = "Freddie",
                                       Value = "Mercury"
                                   });
            Assert.Single(sut.ArgumentValues);
        }
    }
}
