using System;
using System.Collections.Generic;

namespace NoWoL.TestingUtilities.ObjectCreators
{
    public class ValueTypeCreator : IObjectCreator
    {
        internal const string DefaultStringValue = "SomeValue";

        public bool CanHandle(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsValueType || type == typeof(string);
        }

        public object Create(Type type, ICollection<IObjectCreator> objectCreators)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type == typeof(string))
            {
                return DefaultStringValue;
            }

            if (type.IsValueType)
            {
                return type.GetDefault();
            }

            throw new NotSupportedException("Expecting a string or value type however received " + type.FullName);
        }
    }
}