using System;
using System.Collections.Generic;

namespace NoWoL.TestingUtilities.ObjectCreators
{
    public class GenericIEnumerableCreator : IObjectCreator
    {
        public bool CanHandle(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

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
                var itemType = type.GetGenericArguments()[0];
                return CreatorHelpers.CreateArray(itemType, objectCreators);
            }

            throw new NotSupportedException("Expecting an IEnumerable<> type however received " + type.FullName);
        }
    }
}