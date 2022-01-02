using System;
using System.Reflection;
using Moq;

namespace NoWoL.TestingUtilities
{
    /// <summary>
    /// Provides helper methods for <see cref="Type"/>
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Get the default value for the specified type
        /// </summary>
        /// <param name="type">Type to get the default value of</param>
        /// <returns>The default value of the type</returns>
        public static object GetDefault(this Type type)
        {
            return typeof(TypeExtensions).GetMethod(nameof(GetDefaultGeneric), BindingFlags.Static | BindingFlags.Public)!.MakeGenericMethod(type).Invoke(null, null);
        }

        /// <summary>
        /// Get the default value for the specified type
        /// </summary>
        /// <typeparam name="T">Type to get the default value of</typeparam>
        /// <returns>The default value of the type</returns>
        public static T GetDefaultGeneric<T>()
        {
            return default;
        }

        /// <summary>
        /// Creates a Mock&lt;&gt; instance for the specified type
        /// </summary>
        /// <param name="type">Type to mock</param>
        /// <param name="behavior">Mock behavior</param>
        /// <returns>A mocked object</returns>
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