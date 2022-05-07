using NoWoL.TestingUtilities.Expressions;
using Xunit;

namespace NoWoL.TestingUtilities.Tests.Expressions
{
    public class ExpressionArgumentInfoTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void SettingTheNameAllowsUsToReadItBack()
        {
            var sut = new ExpressionArgumentInfo
                      {
                          Name = "Freddie"
                      };
            Assert.Equal("Freddie",
                         sut.Name);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SettingTheValueAllowsUsToReadItBack()
        {
            var sut = new ExpressionArgumentInfo
                      {
                          Value = "Freddie"
                      };
            Assert.Equal("Freddie",
                         sut.Value);
        }
    }
}