using System;
using System.Reflection;

namespace NoWoL.TestingUtilities.ExpectedExceptions
{
    public class ExpectedNoException : IExpectedException
    {
        internal const string MissingException = "An exception was thrown when no exception was expected";

        public virtual string Name => nameof(ExpectedNoException);

        public virtual bool Evaluate(string paramName, Exception ex, out string additionalReason)
        {
            if (paramName == null)
            {
                throw new ArgumentNullException(nameof(paramName));
            }

            if (ex == null)
            {
                additionalReason = null;

                return true;
            }

            additionalReason = MissingException;

            return false;
        }

        public virtual object GetInvalidParameterValue(ParameterInfo param, object defaultValue)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            return defaultValue;
        }
    }
}