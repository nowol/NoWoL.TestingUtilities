using System;
using NoWoL.TestingUtilities.Exceptions;
// ReSharper disable CheckNamespace
#pragma warning disable IDE0079 // Remove unnecessary suppression

namespace NoWoL.TestingUtilities
{
    [Serializable]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class ArgumentRuleException : Exception
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Creates an instance of the <see cref="ArgumentRuleException"/> class.
        /// </summary>
        public ArgumentRuleException()
        {}
        
        /// <summary>
        /// Creates an instance of the <see cref="ArgumentRuleException"/> class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        public ArgumentRuleException(string message) 
            : base(message)
        {}

        /// <summary>
        /// Creates an instance of the <see cref="ArgumentRuleException"/> class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        /// <param name="innerException">Optional inner exception</param>
        public ArgumentRuleException(string message, Exception innerException)
            : base(message, innerException)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentRuleException"/> class.
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        protected ArgumentRuleException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Helper method to create the exception
        /// </summary>
        /// <param name="innerException">Optional inner exception</param>
        /// <returns>An instance of the <see cref="ArgumentRuleException"/> exception</returns>
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        public static ArgumentRuleException Create(string ruleName, string paramName, string additionalReason, Exception innerException = null)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            return new ArgumentRuleException($"Rule '{ruleName}' for parameter '{paramName}' was not respected.{ExceptionFormatters.AddSpaceWhenRequiredFormatter(additionalReason)}", innerException);
#pragma warning restore CA1062 // Validate arguments of public methods
        }
    }
}
namespace NoWoL.TestingUtilities
{
    [Serializable]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class UnconfiguredArgumentsException : Exception
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Creates an instance of the <see cref="UnconfiguredArgumentsException"/> class.
        /// </summary>
        public UnconfiguredArgumentsException()
        {}
        
        /// <summary>
        /// Creates an instance of the <see cref="UnconfiguredArgumentsException"/> class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        public UnconfiguredArgumentsException(string message) 
            : base(message)
        {}

        /// <summary>
        /// Creates an instance of the <see cref="UnconfiguredArgumentsException"/> class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        /// <param name="innerException">Optional inner exception</param>
        public UnconfiguredArgumentsException(string message, Exception innerException)
            : base(message, innerException)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="UnconfiguredArgumentsException"/> class.
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        protected UnconfiguredArgumentsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Helper method to create the exception
        /// </summary>
        /// <param name="innerException">Optional inner exception</param>
        /// <returns>An instance of the <see cref="UnconfiguredArgumentsException"/> exception</returns>
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        public static UnconfiguredArgumentsException Create(System.Collections.Generic.IEnumerable<string> missingArguments, Exception innerException = null)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            return new UnconfiguredArgumentsException($"The following arguments have not been configured: {ExceptionFormatters.JoinCommaFormatter(missingArguments)}.", innerException);
#pragma warning restore CA1062 // Validate arguments of public methods
        }
    }
}
namespace NoWoL.TestingUtilities
{
    [Serializable]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class MissingObjectCreatorException : Exception
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Creates an instance of the <see cref="MissingObjectCreatorException"/> class.
        /// </summary>
        public MissingObjectCreatorException()
        {}
        
        /// <summary>
        /// Creates an instance of the <see cref="MissingObjectCreatorException"/> class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        public MissingObjectCreatorException(string message) 
            : base(message)
        {}

        /// <summary>
        /// Creates an instance of the <see cref="MissingObjectCreatorException"/> class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        /// <param name="innerException">Optional inner exception</param>
        public MissingObjectCreatorException(string message, Exception innerException)
            : base(message, innerException)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingObjectCreatorException"/> class.
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        protected MissingObjectCreatorException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Helper method to create the exception
        /// </summary>
        /// <param name="innerException">Optional inner exception</param>
        /// <returns>An instance of the <see cref="MissingObjectCreatorException"/> exception</returns>
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        public static MissingObjectCreatorException Create(Type type, Exception innerException = null)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            return new MissingObjectCreatorException($"Could not find an IObjectCreator for {ExceptionFormatters.TypeFullNameFormatter(type)}.", innerException);
#pragma warning restore CA1062 // Validate arguments of public methods
        }
    }
}
namespace NoWoL.TestingUtilities
{
    [Serializable]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class UnsupportedInvalidTypeException : Exception
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Creates an instance of the <see cref="UnsupportedInvalidTypeException"/> class.
        /// </summary>
        public UnsupportedInvalidTypeException()
        {}
        
        /// <summary>
        /// Creates an instance of the <see cref="UnsupportedInvalidTypeException"/> class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        public UnsupportedInvalidTypeException(string message) 
            : base(message)
        {}

        /// <summary>
        /// Creates an instance of the <see cref="UnsupportedInvalidTypeException"/> class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        /// <param name="innerException">Optional inner exception</param>
        public UnsupportedInvalidTypeException(string message, Exception innerException)
            : base(message, innerException)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedInvalidTypeException"/> class.
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        protected UnsupportedInvalidTypeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Helper method to create the exception
        /// </summary>
        /// <param name="innerException">Optional inner exception</param>
        /// <returns>An instance of the <see cref="UnsupportedInvalidTypeException"/> exception</returns>
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        public static UnsupportedInvalidTypeException Create(Type type, Exception innerException = null)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            return new UnsupportedInvalidTypeException($"Unable to generate an invalid value for type '{ExceptionFormatters.TypeFullNameFormatter(type)}'.", innerException);
#pragma warning restore CA1062 // Validate arguments of public methods
        }
    }
}
namespace NoWoL.TestingUtilities
{
    [Serializable]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class UnsupportedTypeException : Exception
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Creates an instance of the <see cref="UnsupportedTypeException"/> class.
        /// </summary>
        public UnsupportedTypeException()
        {}
        
        /// <summary>
        /// Creates an instance of the <see cref="UnsupportedTypeException"/> class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        public UnsupportedTypeException(string message) 
            : base(message)
        {}

        /// <summary>
        /// Creates an instance of the <see cref="UnsupportedTypeException"/> class.
        /// </summary>
        /// <param name="message">Message of the exception</param>
        /// <param name="innerException">Optional inner exception</param>
        public UnsupportedTypeException(string message, Exception innerException)
            : base(message, innerException)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedTypeException"/> class.
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        protected UnsupportedTypeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}

