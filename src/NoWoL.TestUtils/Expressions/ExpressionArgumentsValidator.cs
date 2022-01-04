using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NoWoL.TestingUtilities.Expressions
{
    /// <summary>
    /// This class will produce an <see cref="ArgumentsValidator"/> from an expression
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExpressionArgumentsValidator<T>
        where T: class
    {
        private readonly T _targetObject;
        private readonly IEnumerable<IObjectCreator> _objectCreators;
        private readonly List<IExpectedExceptionRule[]> _parameterRules = new();

        /// <summary>
        /// Creates an instance of the <see cref="ExpressionArgumentsValidator{T}"/> class.
        /// </summary>
        /// <param name="targetObject">Object to test</param>
        /// <param name="objectCreators">Optional object creators used to create types during testing</param>
        public ExpressionArgumentsValidator(T targetObject, IEnumerable<IObjectCreator> objectCreators)
        {
            _targetObject = targetObject ?? throw new ArgumentNullException(nameof(targetObject));
            _objectCreators = objectCreators;
        }

        /// <summary>
        /// Set the rules for an argument
        /// </summary>
        /// <typeparam name="TResult">Type of the argument</typeparam>
        /// <param name="rules">List of rules for the argument</param>
        /// <returns>The default value of type <typeparamref name="TResult"/></returns>
        public TResult For<TResult>(params IExpectedExceptionRule[] rules)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            if (rules.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.",
                                            nameof(rules));
            }

            _parameterRules.Add(rules);

            return default;
        }

        /// <summary>
        /// Create an <see cref="ArgumentsValidator"/> for the method specified by the expression
        /// </summary>
        /// <typeparam name="TResult">Return type of the method</typeparam>
        /// <param name="expr">Expression to validate</param>
        /// <returns>A configured instance of <see cref="ArgumentsValidator"/> that you can call Validate/ValidateAsync on.</returns>
        public ArgumentsValidator Setup<TResult>(Expression<Func<T, TResult>> expr)
        {
            var analyzer = new ExpressionArgumentsAnalyzer();
            var expInfo = analyzer.Analyze(expr);

            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(_targetObject,
                                                                                 expInfo.Method,
                                                                                 objectCreators: _objectCreators);

            for (var i = 0; i < expInfo.ArgumentValues.Count; i++)
            {
                var argValue = expInfo.ArgumentValues[i];
                var paramRules = _parameterRules[i];

                validator.SetupParameter(argValue.Name,
                                         paramRules);
            }

            return validator;
        }
    }
}