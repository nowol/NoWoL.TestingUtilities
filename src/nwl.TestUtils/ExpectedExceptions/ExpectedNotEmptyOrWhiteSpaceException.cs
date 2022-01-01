using System;
using System.Reflection;

namespace nwl.TestingUtilities.ExpectedExceptions
{
    public class ExpectedNotEmptyOrWhiteSpaceException : ExpectedExceptionBase
    {
        internal const string DefaultInvalidValue = "   \n   ";

        public override string Name => nameof(ExpectedNotEmptyOrWhiteSpaceException);

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

            return DefaultInvalidValue;
        }
    }
}