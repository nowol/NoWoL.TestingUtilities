using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using NoWoL.TestingUtilities.Expressions;
using Xunit;

namespace NoWoL.TestingUtilities.Tests.Expressions
{
    public class ExpressionArgumentsAnalyzerTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void AnalyzeThrowsIfExpressionIsNotACallExpression()
        {
            var sut = new ExpressionArgumentsAnalyzer();
            var ex = Assert.Throws<ArgumentException>(() => sut.Analyze<TestClass, int>(x => x.Number));
            Assert.Equal("The expression 'x.Number' of type 'PropertyExpression' is unsupported",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AnalyzeFuncThrowsIfExpressionIsNull()
        {
            var sut = new ExpressionArgumentsAnalyzer();
            var ex = Assert.Throws<ArgumentNullException>(() => sut.Analyze<TestClass, int>((Expression<Func<TestClass, int>>)null));
            Assert.Equal("expression",
                         ex.ParamName);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AnalyzeActionThrowsIfExpressionIsNull()
        {
            var sut = new ExpressionArgumentsAnalyzer();
            var ex = Assert.Throws<ArgumentNullException>(() => sut.Analyze<TestClass>((Expression<Action<TestClass>>)null));
            Assert.Equal("expression",
                         ex.ParamName);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AnalyzeReturnsTheMethodArgumentsForNonVoidMethod()
        {
            var sut = new ExpressionArgumentsAnalyzer();
            var info = sut.Analyze<TestClass, int>(x => x.SomeMethod(23, "Text", 25));

            Assert.Equal(nameof(TestClass.SomeMethod),
                         info.MethodName);
            Assert.Equal(typeof(TestClass).GetMethod(nameof(TestClass.SomeMethod)),
                         info.Method);
            Assert.Equal(3,
                         info.ArgumentValues.Count);
            Assert.Equal("i", info.ArgumentValues[0].Name);
            Assert.Equal(23, info.ArgumentValues[0].Value);
            Assert.Equal("s", info.ArgumentValues[1].Name);
            Assert.Equal("Text", info.ArgumentValues[1].Value);
            Assert.Equal("d", info.ArgumentValues[2].Name);
            Assert.Equal(25d, info.ArgumentValues[2].Value);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void AnalyzeReturnsTheMethodArgumentsForVoidMethod()
        {
            var sut = new ExpressionArgumentsAnalyzer();
            var info = sut.Analyze<TestClass>(x => x.SomeVoidMethod(23, "Text", 25));

            Assert.Equal(nameof(TestClass.SomeVoidMethod),
                         info.MethodName);
            Assert.Equal(typeof(TestClass).GetMethod(nameof(TestClass.SomeVoidMethod)),
                         info.Method);
            Assert.Equal(3,
                         info.ArgumentValues.Count);
            Assert.Equal("i", info.ArgumentValues[0].Name);
            Assert.Equal(23, info.ArgumentValues[0].Value);
            Assert.Equal("s", info.ArgumentValues[1].Name);
            Assert.Equal("Text", info.ArgumentValues[1].Value);
            Assert.Equal("d", info.ArgumentValues[2].Name);
            Assert.Equal(25d, info.ArgumentValues[2].Value);
        }

        [ExcludeFromCodeCoverage]
        private class TestClass
        {
            public int Number { get; set; }

            public int SomeMethod(int i, string s, double d = 0)
            {
                return default;
            }

            public void SomeVoidMethod(int i, string s, double d = 0)
            {
            }
        }
    }
}
