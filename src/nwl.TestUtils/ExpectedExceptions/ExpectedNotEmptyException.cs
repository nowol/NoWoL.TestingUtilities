using System;
using System.Collections.Generic;
using System.Reflection;
using nwl.TestingUtilities.ObjectCreators;

namespace nwl.TestingUtilities.ExpectedExceptions
{
    public class ExpectedNotEmptyException : ExpectedExceptionBase
    {
        public override string Name => nameof(ExpectedNotEmptyException);

        public override bool Evaluate(string paramName, Exception ex, out string additionalReason)
        {
            if (!base.Evaluate(paramName, ex, out additionalReason))
            {
                return false;
            }
            else if (ex is ArgumentException ane)
            {
                if (!String.Equals(ane.ParamName, paramName))
                {
                    additionalReason = $"An ArgumentException for the parameter '{paramName}' was expected however the exception is for parameter '{ane.ParamName}'";
                    return false;
                }

                additionalReason = null;

                return true; // validate message content in an extensible way? e.g.: message contains "was empty"
            }
            else
            {
                additionalReason = null;
                return false;
            }
        }

        public override object GetInvalidParameterValue(ParameterInfo param, object defaultValue)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            if (param.ParameterType == typeof(string))
            {
                return "";
            }

            if (param.ParameterType.IsArray)
            {
                return Array.CreateInstance(param.ParameterType.GetElementType()!, 0);
            }

            if (param.ParameterType.IsGenericType)
            {
                if (param.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    var itemType = param.ParameterType.GetGenericArguments()[0];
                    return Array.CreateInstance(itemType, 0);
                }
                else if (param.ParameterType.GetGenericTypeDefinition() == typeof(ICollection<>)
                         || param.ParameterType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    var listType = typeof(List<>).MakeGenericType(param.ParameterType.GenericTypeArguments[0]);
                    return Activator.CreateInstance(listType);
                }
                else if (param.ParameterType.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                         ||
                         param.ParameterType.GetGenericTypeDefinition() == typeof(Dictionary<,>)
                         )
                {
                    return GenericDictionaryCreator.CreateDictionaryFromType(param.ParameterType);
                }
                else if (param.ParameterType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var listType = typeof(List<>).MakeGenericType(param.ParameterType.GenericTypeArguments[0]);
                    return Activator.CreateInstance(listType);
                }
            }

            throw new NotSupportedException("Unknown type: " + param.ParameterType.FullName);
        }
    }
}