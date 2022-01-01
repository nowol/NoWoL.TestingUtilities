using System;
using System.Collections.Generic;

namespace NoWoL.TestingUtilities
{
    public interface IObjectCreator
    {
        bool CanHandle(Type type);

        object Create(Type type, ICollection<IObjectCreator> objectCreators);
    }
}