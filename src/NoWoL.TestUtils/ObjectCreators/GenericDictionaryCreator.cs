using System;
using System.Collections.Generic;

namespace NoWoL.TestingUtilities.ObjectCreators
{
    /// <summary>
    /// Provides a way to create dictionary
    /// </summary>
    public class GenericDictionaryCreator : IObjectCreator
    {
        /// <summary>
        /// Determines whether this instance can create the specified object type.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <returns><c>true</c> if this instance can create the specified object type; otherwise, <c>false</c>.</returns>
        public bool CanHandle(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return IsDictionary(type);
        }

        internal static bool IsDictionary(Type type)
        {
            return type.IsGenericType 
                   && 
                   (
                       type.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                       ||
                       type.GetGenericTypeDefinition() == typeof(Dictionary<,>)
                   );
        }

        /// <summary>
        /// Create an instance of the specified object type.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <param name="objectCreators">A list of <see cref="IObjectCreator"/> to handle creation of sub objects.</param>
        /// <returns>The created object.</returns>
        public object Create(Type type, ICollection<IObjectCreator> objectCreators)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (objectCreators == null)
            {
                throw new ArgumentNullException(nameof(objectCreators));
            }

            if (CanHandle(type))
            {
                var dict = CreateDictionaryFromType(type);

                var keyType = type.GetGenericArguments()[0];
                var key = CreatorHelpers.CreateItemFromType(keyType, objectCreators);
                
                var valueType = type.GetGenericArguments()[1];
                var value = CreatorHelpers.CreateItemFromType(valueType, objectCreators);

                var addMethod = type.GetMethod(nameof(IDictionary<int, int>.Add),
                                               new[] { keyType, valueType });
                addMethod!.Invoke(dict,
                                  new[]
                                  {
                                      key,
                                      value
                                  });

                return dict;
            }

            throw new UnsupportedTypeException("Expecting a Dictionary type however received " + type.FullName);
        }

        internal static object CreateDictionaryFromType(Type type)
        {
            var keyType = type.GetGenericArguments()[0];
            var valueType = type.GetGenericArguments()[1];

            var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            var dict = Activator.CreateInstance(dictType);

            return dict;
        }
    }
}