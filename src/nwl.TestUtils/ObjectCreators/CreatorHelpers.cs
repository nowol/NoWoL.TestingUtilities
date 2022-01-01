using System;
using System.Collections;
using System.Collections.Generic;

namespace nwl.TestingUtilities.ObjectCreators
{
    internal static class CreatorHelpers
    {
        internal static object CreateList(Type type, ICollection<IObjectCreator> objectCreators)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (objectCreators == null)
            {
                throw new ArgumentNullException(nameof(objectCreators));
            }

            var listType = typeof(List<>).MakeGenericType(type);
            var list = Activator.CreateInstance(listType) as IList;
            list!.Add(CreateItemFromType(type, objectCreators));
            return list;
        }

        internal static object CreateArray(Type type, ICollection<IObjectCreator> objectCreators)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (objectCreators == null)
            {
                throw new ArgumentNullException(nameof(objectCreators));
            }

            var array = Array.CreateInstance(type, 1);
            array.SetValue(CreateItemFromType(type, objectCreators), 0);
            return array;
        }

        internal static object CreateItemFromType(Type type, ICollection<IObjectCreator> objectCreators)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (objectCreators == null)
            {
                throw new ArgumentNullException(nameof(objectCreators));
            }

            if (TryCreateObject(type, objectCreators, out var result))
            {
                return result;
            }

            throw new NotSupportedException("No object creators have been registered to handle type '" + type.FullName + "'");
        }

        internal static bool TryCreateObject(Type type, ICollection<IObjectCreator> objectCreators, out object result)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (objectCreators == null)
            {
                throw new ArgumentNullException(nameof(objectCreators));
            }

            foreach (var objCreator in objectCreators)
            {
                if (objCreator.CanHandle(type))
                {
                    result = objCreator.Create(type, objectCreators);
                    return true;
                }
            }

            result = null;

            return false;
        }
    }
}