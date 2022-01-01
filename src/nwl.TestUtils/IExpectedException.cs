using System;
using System.Reflection;

namespace nwl.TestingUtilities
{
    public interface IExpectedException
    {
        string Name { get; }

        bool Evaluate(string paramName, Exception ex, out string additionalReason);

        object GetInvalidParameterValue(ParameterInfo param, object defaultValue);
    }
}