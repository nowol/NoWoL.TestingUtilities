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
    /// Validates the arguments of a method or constructor
    /// </summary>
    public class ArgumentsValidator
    {
        private readonly object _targetObject;
        private readonly MethodBase _method;
        private readonly IObjectCreator[] _objectCreators;
        private readonly object[] _methodArguments;
        private readonly Dictionary<string, IExpectedExceptionRule[]> _expectedExceptions = new();
        private readonly ParameterInfo[] _parameters;

        /// <summary>
        /// Creates an instance of the <see cref="ArgumentsValidator"/> class.
        /// </summary>
        /// <param name="targetObject">Object to test. Can be null if testing a static method.</param>
        /// <param name="method">Method to test</param>
        /// <param name="methodArguments">Optional arguments used for testing</param>
        /// <param name="objectCreators">Optional object creators used to create types during testing</param>
        public ArgumentsValidator(object targetObject, MethodBase method, object[] methodArguments, IObjectCreator[] objectCreators)
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
            _methodArguments = methodArguments;

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
        public static IArgumentsValidationRules DefaultRules { get; } = GetDefaultRules();

        private static IArgumentsValidationRules GetDefaultRules()
        {
            return new ArgumentsValidationRules
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
        /// <param name="paramName">Parameter to validate</param>
        /// <param name="rules">Validation rules</param>
        /// <returns>This instance of <see cref="ArgumentsValidator"/> to allow chaining.</returns>
        public ArgumentsValidator SetupParameter(string paramName, params IExpectedExceptionRule[] rules)
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

            if (!_parameters.Any(x => String.Equals(x.Name, paramName, StringComparison.Ordinal)))
            {
                throw new ArgumentException($"Parameter '{paramName}' does not exists on the method.", nameof(paramName));
            }

            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            if (rules.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.",
                                            nameof(rules));
            }

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
        /// <param name="paramName">Parameter to validate</param>
        /// <param name="rules">Validation rules</param>
        /// <returns>This instance of <see cref="ArgumentsValidator"/> to allow chaining.</returns>
        public ArgumentsValidator UpdateParameter(string paramName, params IExpectedExceptionRule[] rules)
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

            if (!_parameters.Any(x => String.Equals(x.Name, paramName, StringComparison.Ordinal)))
            {
                throw new ArgumentException($"Parameter '{paramName}' does not exists on the method.", nameof(paramName));
            }

            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            if (rules.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.",
                                            nameof(rules));
            }

            if (!_expectedExceptions.ContainsKey(paramName))
            {
                throw new KeyNotFoundException($"The given key '{paramName}' was not present in the dictionary.");
            }

            _expectedExceptions[paramName] = rules;

            return this;
        }

        /// <summary>
        /// Get the parameter rules
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

            object[] defaultParameters = GetDefaultParameters();

            for (var i = 0; i < _parameters.Length; i++)
            {
                var param = _parameters[i];
                var rules = _expectedExceptions[param.Name];

                foreach (var rule in rules)
                {
                    var arguments = defaultParameters.ToArray(); // copy array to avoid recreating the default parameters every time
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
                        throw ArgumentRuleException.Create(rule.Name, param.Name, additionalReason);
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

            object[] defaultParameters = GetDefaultParameters();

            for (var i = 0; i < _parameters.Length; i++)
            {
                var param = _parameters[i];
                var rules = _expectedExceptions[param.Name];

                foreach (var rule in rules)
                {
                    var arguments = defaultParameters.ToArray(); // copy array to avoid recreating the default parameters every time
                    arguments[i] = rule.GetInvalidParameterValue(param, arguments[i]);

                    Exception result = null;
                    bool nonAsyncWasCalled = false;

                    try
                    {
                        // the flow is a bit wonky here but it needs to be like this to help with the code coverage

                        if (_method is not MethodInfo mi)
                        {
                            nonAsyncWasCalled = true;
                            throw new NotSupportedException("The requested method is not compatible with async/await. Please call the non async Validate method.");
                        }

                        if (!TryGetAwaitableTask(mi,
                                                 arguments,
                                                 out var task))
                        {
                            nonAsyncWasCalled = true;
                            throw new NotSupportedException("The requested method does not return a Task. Please call the non async Validate method.");
                        }

                        await task.ConfigureAwait(false);
                    }
                    catch (Exception ex) when (!nonAsyncWasCalled)
                    {
                        result = ex;
                    }

                    if (!rule.Evaluate(param.Name, result, out string additionalReason))
                    {
                        throw ArgumentRuleException.Create(rule.Name, param.Name, additionalReason);
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

        private object[] GetDefaultParameters()
        {
            return _methodArguments ?? CreateDefaultParameters();
        }

        private void ValidateConfiguredExceptions()
        {
            if (_expectedExceptions.Count == 0)
            {
                throw new InvalidOperationException("No arguments were configured for validation. Call SetupAll or SetupParameter before calling Validate/ValidateAsync.");
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

        private object[] CreateDefaultParameters()
        {
            var result = new List<object>();

            foreach (var parameterInfo in _method.GetParameters())
            {
                bool handled = CreatorHelpers.TryCreateObject(parameterInfo.ParameterType, _objectCreators, out var createdObject);
                if (!handled)
                {
                    throw  new Exception("Could not find an IObjectCreator for " + parameterInfo.ParameterType.FullName);
                }
                else
                {
                    result.Add(createdObject);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Automatically configure the arguments' rules using the rules specified by <paramref name="validationRules"/>. Calling this will replace any previous rules.
        /// </summary>
        /// <param name="validationRules">Validation rules to apply</param>
        /// <returns>This instance of <see cref="ArgumentsValidator"/> to allow chaining.</returns>
        public ArgumentsValidator SetupAll(IArgumentsValidationRules validationRules)
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

        private IExpectedExceptionRule[] GetRulesForDataType(Type dataType, IArgumentsValidationRules validationRules)
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