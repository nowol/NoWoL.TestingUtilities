using System;
using System.Reflection;

namespace NoWoL.TestingUtilities.ExpectedExceptions
{
    public abstract class ExpectedExceptionBase : IExpectedException
    {
        internal const string NoExceptionMessage = "An exception was expected but none happened.";

        public abstract string Name { get; }

        public virtual bool Evaluate(string paramName, Exception ex, out string additionalReason)
        {
            if (paramName == null)
            {
                throw new ArgumentNullException(nameof(paramName));
            }

            if (ex == null)
            {
                additionalReason = NoExceptionMessage;
                return false;
            }

            additionalReason = null;

            return true;
        }

        public abstract object GetInvalidParameterValue(ParameterInfo param, object defaultValue);
    }
}