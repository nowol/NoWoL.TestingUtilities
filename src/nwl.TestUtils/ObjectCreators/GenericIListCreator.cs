using System;
using System.Collections.Generic;

namespace nwl.TestingUtilities.ObjectCreators
{
    public class GenericIListCreator : IObjectCreator
    {
        public bool CanHandle(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>);
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
                return CreatorHelpers.CreateList(itemType, objectCreators);
            }

            throw new NotSupportedException("Expecting an IList<> type however received " + type.FullName);
        }
    }
}