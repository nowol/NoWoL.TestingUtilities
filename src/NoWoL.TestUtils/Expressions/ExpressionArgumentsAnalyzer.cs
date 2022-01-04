using System;
using System.Linq.Expressions;

namespace NoWoL.TestingUtilities.Expressions
{
    /// <summary>
    /// Helper class to extract argument values from an expression
    /// </summary>
    internal class ExpressionArgumentsAnalyzer
    {
        /// <summary>
        /// Analyze the expression and extract its method call arguments
        /// </summary>
        /// <typeparam name="T">Type of the object that the expression refers to.</typeparam>
        /// <typeparam name="TResult">Return type of the method of the expression</typeparam>
        /// <param name="expression">Expression to analyze</param>
        /// <returns>An <see cref="ExpressionMethodInfo"/> holding the information about the call expression</returns>
        public ExpressionMethodInfo Analyze<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            return AnalyzeLambdaExpression(expression);
        }

        /// <summary>
        /// Analyze the expression and extract its method call arguments
        /// </summary>
        /// <typeparam name="T">Type of the object that the expression refers to.</typeparam>
        /// <param name="expression">Expression to analyze</param>
        /// <returns>An <see cref="ExpressionMethodInfo"/> holding the information about the call expression</returns>
        public ExpressionMethodInfo Analyze<T>(Expression<Action<T>> expression)
        {
            return AnalyzeLambdaExpression(expression);
        }

        private ExpressionMethodInfo AnalyzeLambdaExpression(LambdaExpression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (expression.Body is not MethodCallExpression callExpression)
            {
                throw new ArgumentException($"The expression '{expression.Body}' of type '{expression.Body.GetType().Name}' is unsupported");
            }

            var info = new ExpressionMethodInfo
                       {
                           Method = callExpression.Method,
                           MethodName = callExpression.Method.Name
                       };

            var ps = info.Method.GetParameters();

            for (int i = 0; i < callExpression.Arguments.Count; i++)
            {
                var arg = callExpression.Arguments[i];

                var delegateType = typeof(Func<>).MakeGenericType(arg.Type);
                var compiledDelegate = Expression.Lambda(delegateType,
                                                         arg).Compile();
                var value = compiledDelegate.DynamicInvoke();
                info.ArgumentValues.Add(new ExpressionArgumentInfo
                                        {
                                            Name = ps[i].Name,
                                            Value = value
                                        });
            }

            return info;
        }
    }
}