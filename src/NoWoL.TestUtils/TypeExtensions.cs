using System;
using System.Reflection;
using Moq;

namespace NoWoL.TestingUtilities
{
    public static class TypeExtensions
    {
        public static object GetDefault(this Type t)
        {
            return typeof(TypeExtensions).GetMethod(nameof(GetDefaultGeneric), BindingFlags.Static | BindingFlags.Public)!.MakeGenericMethod(t).Invoke(null, null);
        }

        public static T GetDefaultGeneric<T>()
        {
            return default(T);
        }

        public static object GetObjectMock(this Type type, MockBehavior behavior = MockBehavior.Strict)
        {
            var method = typeof(TypeExtensions).GetMethod(nameof(TypeExtensions.GetObjectMockGeneric), BindingFlags.Static | BindingFlags.NonPublic);

            var genericMethod = method!.MakeGenericMethod(type);

            return genericMethod.Invoke(null,
                                        new object[]
                                        {
                                            behavior
                                        });
        }

        private static object GetObjectMockGeneric<T>(MockBehavior behavior)
            where T : class
        {
            return new Mock<T>(behavior).Object;
        }
    }
}