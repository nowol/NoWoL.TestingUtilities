using System;
using System.Reflection;

namespace NoWoL.TestingUtilities.ExpectedExceptions
{
    public class ExpectedNotNullException : ExpectedExceptionBase
    {
        public override string Name => nameof(ExpectedNotNullException);

        public override bool Evaluate(string paramName, Exception ex, out string additionalReason)
        {
            if (!base.Evaluate(paramName, ex, out additionalReason))
            {
                return false;
            }
            else if (ex is ArgumentNullException ane)
            {
                if (!String.Equals(ane.ParamName, paramName))
                {
                    additionalReason = $"An ArgumentNullException for the parameter '{paramName}' was expected however the exception is for parameter '{ane.ParamName}'";
                    return false;
                }

                additionalReason = null;

                return true;
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

            return null;
        }
    }
}