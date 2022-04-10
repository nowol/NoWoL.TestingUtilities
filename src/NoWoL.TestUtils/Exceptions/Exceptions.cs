using System;
using System.Collections.Generic;
using System.Text;
using NoWoL.SourceGenerators;

// ReSharper disable once CheckNamespace
namespace NoWoL.TestingUtilities
{
    /// <summary>The exception that is thrown when a parameter's rule was not respected.</summary>
    [ExceptionGenerator("Rule '{string ruleName}' for parameter '{string paramName}' was not respected.{<NoWoL.TestingUtilities.Exceptions.ExceptionFormatters.AddSpaceWhenRequiredFormatter>string additionalReason}")]
    public partial class ParameterRuleException { }

    /// <summary>The exception that is thrown when some arguments were not configured.</summary>
    [ExceptionGenerator("The following arguments have not been configured: {<NoWoL.TestingUtilities.Exceptions.ExceptionFormatters.JoinCommaFormatter>System.Collections.Generic.IEnumerable<string> missingArguments}.")]
    public partial class UnconfiguredArgumentsException { }

    /// <summary>The exception that is thrown when an <see cref="IObjectCreator"/> have not been defined for a type,</summary>
    [ExceptionGenerator("Could not find an IObjectCreator for {<NoWoL.TestingUtilities.Exceptions.ExceptionFormatters.TypeFullNameFormatter>System.Type type}.")]
    public partial class MissingObjectCreatorException { }

    /// <summary>The exception that is thrown when it is impossible to generate a value for a type,</summary>
    [ExceptionGenerator("Unable to generate an invalid value for type '{<NoWoL.TestingUtilities.Exceptions.ExceptionFormatters.TypeFullNameFormatter>System.Type type}'.")]
    public partial class UnsupportedInvalidTypeException { }

    /// <summary>The exception that is thrown when a type is not supported.</summary>
    [ExceptionGenerator()]
    public partial class UnsupportedTypeException { }

    /// <summary>The exception that is thrown when you cannot use SetParameterValue.</summary>
    [ExceptionGenerator("Your validator is already using 'methodParameters' to define default values. Set your parameter value using 'methodParameters' instead of calling 'SetParameterValue'.")]
    public partial class UseMethodParametersValuesInsteadException { }
}
