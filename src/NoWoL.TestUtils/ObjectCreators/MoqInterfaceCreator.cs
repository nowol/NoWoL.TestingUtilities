using System;
using System.Collections.Generic;
using Moq;

namespace NoWoL.TestingUtilities.ObjectCreators
{
    public class MoqInterfaceCreator : IObjectCreator
	{
		public bool CanHandle(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsInterface;
        }

		public object Create(Type type, ICollection<IObjectCreator> objectCreators)
		{
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (CanHandle(type))
            {
				return type.GetObjectMock(MockBehavior.Loose);
			}

            throw new NotSupportedException("Expecting an interface however received " + type.FullName);
		}
	}
}
