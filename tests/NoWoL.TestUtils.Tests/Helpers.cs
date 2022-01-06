using System;
using System.Linq;
using System.Reflection;
using Moq;
using Xunit;

namespace NoWoL.TestingUtilities.Tests
{
    internal static class TestHelpers
    {
        internal static void AssertType(Type expectedType, object value)
        {
            if (expectedType.IsValueType
                || expectedType == typeof(string))
            {
                Assert.IsType(expectedType,
                              value);
            }
            else
            {
                var getMethod = typeof(Mock).GetMethod(nameof(Mock.Get),
                                                       BindingFlags.Static | BindingFlags.Public);
                var genMethod = getMethod!.MakeGenericMethod(expectedType);

                var underlyingMock = genMethod.Invoke(null,
                                                      new[] { value });

                Assert.True(underlyingMock!.GetType().IsGenericType);
                Assert.Equal(expectedType,
                             underlyingMock.GetType().GenericTypeArguments.Single());
            }
        }
    }
}