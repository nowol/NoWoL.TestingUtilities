using System;
using System.Reflection;

namespace NoWoL.TestingUtilities.ExpectedExceptions
{
    public class ExpectedExceptionWithInvalidValue<T> : ExpectedExceptionBase
    {
        private readonly T _invalidValue;

        public ExpectedExceptionWithInvalidValue(T invalidValue)
        {
            _invalidValue = invalidValue;
        }

        public override string Name => nameof(ExpectedExceptionWithInvalidValue<T>) + " for type " + typeof(T).Name;

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

            return _invalidValue;
        }
    }
}