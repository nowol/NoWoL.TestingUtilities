using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NoWoL.TestingUtilities.ObjectCreators;

namespace NoWoL.TestingUtilities
{
    public class ArgumentsValidator
    {
        private readonly object _targetObject;
        private readonly MethodBase _method;
        private readonly IObjectCreator[] _objectCreators;
        private readonly object[] _methodArguments;
        private readonly Dictionary<string, IExpectedException[]> _expectedExceptions = new Dictionary<string, IExpectedException[]>();
        private readonly ParameterInfo[] _parameters;

        public ArgumentsValidator(object targetObject, MethodBase method, IObjectCreator[] objectCreators, object[] methodArguments)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (objectCreators == null)
            {
                throw new ArgumentNullException(nameof(objectCreators));
            }

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

        public ArgumentsValidator SetupParameter(string paramName, params IExpectedException[] rules)
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

        public void Validate()
        {
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
                        throw new Exception($"Rule '{rule.Name}' for parameter '{param.Name}' was not respected. {additionalReason}");
                    }
                }
            }
        }

        public async Task ValidateAsync()
        {
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
                        throw new Exception($"Rule '{rule.Name}' for parameter '{param.Name}' was not respected. {additionalReason}");
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

        private void ValidateMissingParameters()
        {
            var missingParameters = _parameters.Select(x => x.Name).Except(_expectedExceptions.Keys).ToList();

            if (missingParameters.Count > 0)
            {
                throw new InvalidOperationException($"The following parameters have not been configured: {String.Join(", ", missingParameters)}");
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
                    throw new Exception("Could not find an IObjectCreator for " + parameterInfo.ParameterType.FullName);
                }
                else
                {
                    result.Add(createdObject);
                }
            }

            return result.ToArray();
        }
    }
}