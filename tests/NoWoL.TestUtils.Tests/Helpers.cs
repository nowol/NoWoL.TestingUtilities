using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
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
            else if (ProxyUtil.IsProxyType(value.GetType()))
            {
                Assert.True(expectedType.IsInstanceOfType(value));
            }
        }
    }
}