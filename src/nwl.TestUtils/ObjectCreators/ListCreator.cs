using System;
using System.Collections.Generic;

namespace nwl.TestingUtilities.ObjectCreators
{
    public class ListCreator : IObjectCreator
    {
        public bool CanHandle(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
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
                var itemType = type.GenericTypeArguments[0];
                return CreatorHelpers.CreateList(itemType, objectCreators);
            }

            throw new NotSupportedException("Expecting a List<> type however received " + type.FullName);
        }
    }
}