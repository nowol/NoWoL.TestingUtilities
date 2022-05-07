using System;
using System.Reflection;

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
    }
}