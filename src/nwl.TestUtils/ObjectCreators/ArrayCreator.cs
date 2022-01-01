using System;
using System.Collections.Generic;

namespace nwl.TestingUtilities.ObjectCreators
{
    public class ArrayCreator : IObjectCreator
    {
        public bool CanHandle(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsClass && type.IsArray;
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
                return CreatorHelpers.CreateArray(type.GetElementType(), objectCreators);
            }

            throw new NotSupportedException("Expecting an array type however received " + type.FullName);
        }
    }
}