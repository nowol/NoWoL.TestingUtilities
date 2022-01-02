using System;
using System.Collections.Generic;

namespace NoWoL.TestingUtilities
{
    /// <summary>
    /// Provides a way to create object for a given type
    /// </summary>
    public interface IObjectCreator
    {
        /// <summary>
        /// Determines whether this instance can create the specified object type.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <returns><c>true</c> if this instance can create the specified object type; otherwise, <c>false</c>.</returns>
        bool CanHandle(Type type);

        /// <summary>
        /// Create an instance of the specified object type.
        /// </summary>
        /// <param name="type">Type of the object.</param>
        /// <param name="objectCreators">A list of <see cref="IObjectCreator"/> to handle creation of sub objects.</param>
        /// <returns>The created object.</returns>
        object Create(Type type, ICollection<IObjectCreator> objectCreators);
    }
}