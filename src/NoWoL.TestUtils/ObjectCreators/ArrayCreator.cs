using System;
using System.Collections.Generic;

namespace NoWoL.TestingUtilities.ObjectCreators
{
    /// <summary>
    /// Provides a way to create arrays of a given type
    /// </summary>
    public class ArrayCreator : IObjectCreator
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

            return type.IsClass && type.IsArray;
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
                return CreatorHelpers.CreateArray(type.GetElementType(), objectCreators);
            }

            throw new NotSupportedException("Expecting an array type however received " + type.FullName);
        }
    }
}