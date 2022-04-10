using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NoWoL.TestingUtilities.ExpectedExceptions;
using NoWoL.TestingUtilities.ObjectCreators;

namespace NoWoL.TestingUtilities
{
    /// <summary>
    /// Validates the parameters of a method or constructor
    /// </summary>
    public class ParametersValidator
    {
        private readonly object _targetObject;
        private readonly MethodBase _method;
        private readonly IObjectCreator[] _objectCreators;
        private readonly object[] _methodParameters;
        private readonly Dictionary<string, IExpectedExceptionRule[]> _expectedExceptions = new();
        private readonly ParameterInfo[] _parameters;
        private readonly Dictionary<int, object> _overriddenMethodParameters = new();

        /// <summary>
        /// Creates an instance of the <see cref="ParametersValidator"/> class.
        /// </summary>
        /// <param name="targetObject">Object to test. Can be null if testing a static method.</param>
        /// <param name="method">Method to test</param>
        /// <param name="methodParameters">Optional values used for testing as arguments for the method</param>
        /// <param name="objectCreators">Optional object creators used to create types during testing</param>
        public ParametersValidator(object targetObject, MethodBase method, object[] methodParameters, IObjectCreator[] objectCreators)
        {
#pragma warning disable IDE0016 // Use 'throw' expression
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (objectCreators == null)
            {
                throw new ArgumentNullException(nameof(objectCreators));
            }
#pragma warning restore IDE0016 // Use 'throw' expression

            if (objectCreators.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.",
                                            nameof(objectCreators));
            }

            _targetObject = targetObject;
            _method = method;
            _objectCreators = objectCreators;
            _methodParameters = methodParameters;

            _parameters = _method.GetParameters();

            if (_parameters.Length == 0)
            {
                throw new ArgumentException("The specified method does not have any parameters", nameof(method));
            }
        }

        /// <summary>
        /// Default configuration rules for every parameters.
        /// <remarks>
        /// Strings will be configured to use both NotNull (ArgumentNullException) and NotEmptyOrWhiteSpace (ArgumentException).
        /// Value types will be configured to use None.
        /// Everything else will use NotNull
        /// </remarks>
        /// </summary>
        public static IParametersValidationRules DefaultRules { get; } = GetDefaultRules();

        private static IParametersValidationRules GetDefaultRules()
        {
            return new ParametersValidationRules
                   {
                       StringRules = new []{ ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmptyOrWhiteSpace },
                       ValueTypesRules = new []{ ExpectedExceptionRules.None },
                       InterfacesRules = new []{ ExpectedExceptionRules.NotNull },
                       CollectionTypesRules = new []{ ExpectedExceptionRules.NotNull },
                       OtherTypesRules = new []{ ExpectedExceptionRules.NotNull },
                   };
        }

        /// <summary>
        /// Configures the validation rules for a named parameter
        /// </summary>
        /// <param name="paramName">Name of the parameter to validate</param>
        /// <param name="rules">Validation rules</param>
        /// <returns>This instance of <see cref="ParametersValidator"/> to allow chaining.</returns>
        public ParametersValidator SetupParameter(string paramName, params IExpectedExceptionRule[] rules)
        {
            ValidateParameterNameAndRules(paramName,
                                          rules);

            if (_expectedExceptions.ContainsKey(paramName))
            {
                throw new ArgumentException($"Parameter {paramName} has already been configured", nameof(paramName));
            }

            _expectedExceptions.Add(paramName, rules);

            return this;
        }

        /// <summary>
        /// Update the validation rules for a named parameter
        /// </summary>
        /// <param name="paramName">Name of the parameter to validate</param>
        /// <param name="rules">Validation rules</param>
        /// <returns>This instance of <see cref="ParametersValidator"/> to allow chaining.</returns>
        public ParametersValidator UpdateParameter(string paramName, params IExpectedExceptionRule[] rules)
        {
            ValidateParameterNameAndRules(paramName,
                                          rules);

            if (!_expectedExceptions.ContainsKey(paramName))
            {
                throw new KeyNotFoundException($"The given key '{paramName}' was not present in the dictionary.");
            }

            _expectedExceptions[paramName] = rules;

            return this;
        }

        /// <summary>
        /// Set the value of a parameter for testing the parameters of the target method
        /// </summary>
        /// <remarks>This method is useful because it can allow a developer to avoid specifying all parameter values of a method and only specify the one that is missing an <see cref="IObjectCreator"/>.</remarks>
        /// <param name="paramName">Name of the parameter to validate</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>This instance of <see cref="ParametersValidator"/> to allow chaining.</returns>
        public ParametersValidator SetParameterValue(string paramName, object value)
        {
            ValidateParameterName(paramName);

            if (_methodParameters != null)
            {
                throw UseMethodParametersValuesInsteadException.Create();
            }

            for (var i = 0; i < _parameters.Length; i++)
            {
                var parameterInfo = _parameters[i];

                if (String.Equals(parameterInfo.Name,
                                  paramName,
                                  StringComparison.Ordinal))
                {
                    _overriddenMethodParameters[i] = value;
                    break;
                }
            }

            return this;
        }

        private void ValidateParameterName(string paramName)
        {
            if (paramName == null)
            {
                throw new ArgumentNullException(nameof(paramName));
            }

            if (string.IsNullOrWhiteSpace(paramName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.",
                                            nameof(paramName));
            }

            if (!_parameters.Any(x => String.Equals(x.Name,
                                                    paramName,
                                                    StringComparison.Ordinal)))
            {
                throw new ArgumentException($"Parameter '{paramName}' does not exists on the method.",
                                            nameof(paramName));
            }
        }

        private void ValidateParameterNameAndRules(string paramName, IExpectedExceptionRule[] rules)
        {
            ValidateParameterName(paramName);

            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            if (rules.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.",
                                            nameof(rules));
            }
        }

        /// <summary>
        /// Get the rules of a parameter
        /// </summary>
        /// <param name="paramName">Name of the parameter</param>
        /// <returns>Rules of the parameter</returns>
        public IExpectedExceptionRule[] GetParameterRules(string paramName)
        {
            if (paramName == null)
            {
                throw new ArgumentNullException(nameof(paramName));
            }

            if (string.IsNullOrWhiteSpace(paramName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.",
                                            nameof(paramName));
            }

            return _expectedExceptions[paramName];
        }

        /// <summary>
        /// Apply the validation rules
        /// </summary>
        public void Validate()
        {
            ValidateConfiguredExceptions();
            ValidateMissingParameters();

            object[] defaultArguments = GetDefaultArguments();

            for (var i = 0; i < _parameters.Length; i++)
            {
                var param = _parameters[i];
                var rules = _expectedExceptions[param.Name];

                foreach (var rule in rules)
                {
                    var arguments = defaultArguments.ToArray(); // copy array to avoid recreating the default arguments every time
                    arguments[i] = rule.GetInvalidParameterValue(param, arguments[i]);

                    TargetInvocationException result = null;

                    try
                    {
                        if (_method is ConstructorInfo ctorInfo)
                        {
                            ctorInfo.Invoke(arguments);
                        }
                        else
                        {
                            if (TryGetAwaitableTask((MethodInfo)_method,
                                                    arguments,
                                                    out var task))
                            {
                                throw new NotSupportedException("The requested method does returns a Task. Please call the ValidateAsync method.");
                            }

                            _method.Invoke(_targetObject,
                                           arguments);
                        }
                    }
                    catch (TargetInvocationException ex)
                    {
                        result = ex;
                    }

                    if (!rule.Evaluate(param.Name, result?.InnerException, out string additionalReason))
                    {
                        throw ParameterRuleException.Create(rule.Name, param.Name, additionalReason);
                    }
                }
            }
        }

        /// <summary>
        /// Apply the validation rules
        /// </summary>
        public async Task ValidateAsync()
        {
            ValidateConfiguredExceptions();
            ValidateMissingParameters();

            object[] defaultArguments = GetDefaultArguments();

            for (var i = 0; i < _parameters.Length; i++)
            {
                var param = _parameters[i];
                var rules = _expectedExceptions[param.Name];

                foreach (var rule in rules)
                {
                    var arguments = defaultArguments.ToArray(); // copy array to avoid recreating the default arguments every time
                    arguments[i] = rule.GetInvalidParameterValue(param, arguments[i]);

                    Exception result = null;
                    bool exceptionWasHandled = false;

                    try
                    {
                        // the flow is a bit wonky here but it needs to be like this to help with the code coverage

                        if (_method is not MethodInfo mi)
                        {
                            exceptionWasHandled = true;
                            throw new NotSupportedException("The requested method is not compatible with async/await. Please call the non async Validate method.");
                        }

                        if (!TryGetAwaitableTask(mi,
                                                 arguments,
                                                 out var task))
                        {
                            exceptionWasHandled = true;
                            throw new NotSupportedException("The requested method does not return a Task. Please call the non async Validate method.");
                        }

                        await task.ConfigureAwait(false);
                    }
                    catch (Exception ex) when (!exceptionWasHandled)
                    {
                        result = ex;
                    }

                    if (!rule.Evaluate(param.Name, result, out string additionalReason))
                    {
                        throw ParameterRuleException.Create(rule.Name, param.Name, additionalReason);
                    }
                }
            }
        }

        private bool TryGetAwaitableTask(MethodInfo methodInfo, object[] arguments, out Task task)
        {
            if (methodInfo.ReturnType == typeof(ValueTask))
            {
                task = ((ValueTask)methodInfo.Invoke(_targetObject,
                                                     arguments)).AsTask();

                return true;
            }

            if (methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                var result = methodInfo.Invoke(_targetObject,
                                               arguments);

                var asTaskMethod = methodInfo.ReturnType.GetMethod(nameof(ValueTask<int>.AsTask));

                task = (Task)asTaskMethod!.Invoke(result, null);

                return true;
            }

            if (typeof(Task).IsAssignableFrom(methodInfo.ReturnType)) // handle both Task and Task<>
            {
                task = (Task)methodInfo.Invoke(_targetObject,
                                               arguments);

                return true;
            }

            task = null;

            return false;
        }

        private object[] GetDefaultArguments()
        {
            return _methodParameters ?? CreateDefaultArguments();
        }

        private void ValidateConfiguredExceptions()
        {
            if (_expectedExceptions.Count == 0)
            {
                throw new InvalidOperationException("No parameters were configured for validation. Call SetupAll or SetupParameter before calling Validate/ValidateAsync.");
            }
        }

        private void ValidateMissingParameters()
        {
            var missingParameters = _parameters.Select(x => x.Name).Except(_expectedExceptions.Keys).ToList();

            if (missingParameters.Count > 0)
            {
                throw UnconfiguredArgumentsException.Create(missingParameters);
            }
        }

        private object[] CreateDefaultArguments()
        {
            var result = new List<object>();

            for (var i = 0; i < _method.GetParameters().Length; i++)
            {
                var parameterInfo = _method.GetParameters()[i];

                if (_overriddenMethodParameters.ContainsKey(i))
                {
                    result.Add(_overriddenMethodParameters[i]);
                }
                else
                {
                    bool handled = CreatorHelpers.TryCreateObject(parameterInfo.ParameterType,
                                                                  _objectCreators,
                                                                  out var createdObject);

                    if (!handled)
                    {
                        throw new Exception("Could not find an IObjectCreator for " + parameterInfo.ParameterType.FullName);
                    }
                    else
                    {
                        result.Add(createdObject);
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Automatically configure the parameters' rules using the rules specified by <paramref name="validationRules"/>. Calling this will replace any previous rules.
        /// </summary>
        /// <param name="validationRules">Validation rules to apply</param>
        /// <returns>This instance of <see cref="ParametersValidator"/> to allow chaining.</returns>
        public ParametersValidator SetupAll(IParametersValidationRules validationRules)
        {
            if (validationRules == null)
            {
                throw new ArgumentNullException(nameof(validationRules));
            }

            _expectedExceptions.Clear();

            foreach (var parameterInfo in _parameters)
            {
                var rules = GetRulesForDataType(parameterInfo.ParameterType,
                                                validationRules);

                SetupParameter(parameterInfo.Name,
                               rules ?? new[] { ExpectedExceptionRules.None });
            }

            return this;
        }

        private IExpectedExceptionRule[] GetRulesForDataType(Type dataType, IParametersValidationRules validationRules)
        {
            if (dataType == typeof(string))
            {
                return validationRules.StringRules;
            }

            if (dataType.IsValueType)
            {
                return validationRules.ValueTypesRules;
            }

            if (dataType.IsArray
                || GenericICollectionCreator.IsICollection(dataType)
                || GenericIEnumerableCreator.IsIEnumerable(dataType)
                || GenericListCreator.IsList(dataType)
                || GenericDictionaryCreator.IsDictionary(dataType))
            {
                return validationRules.CollectionTypesRules;
            }

            if (dataType.IsInterface)
            {
                return validationRules.InterfacesRules;
            }

            return validationRules.OtherTypesRules;
        }
    }
}