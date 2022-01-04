using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NoWoL.TestingUtilities.Expressions;
using NoWoL.TestingUtilities.ObjectCreators;

namespace NoWoL.TestingUtilities
{
    /// <summary>
    /// Helper class to create an instance of <see cref="ArgumentsValidator"/>
    /// </summary>
    public static class ArgumentsValidatorHelper
    {
        /// <summary>
        /// Gets the default object creators
        /// </summary>
        public static IObjectCreator[] DefaultCreators { get; } = new IObjectCreator[]
                                                                  {
                                                                      new ArrayCreator(),
                                                                      new GenericDictionaryCreator(),
                                                                      new GenericIEnumerableCreator(),
                                                                      new GenericListCreator(),
                                                                      new GenericICollectionCreator(),
                                                                      new MoqInterfaceCreator(),
                                                                      new ValueTypeCreator()
                                                                  };

        /// <summary>
        /// Creates an instance of <see cref="ArgumentsValidator"/> for the constructor of type <typeparamref name="TConstructor"/>.
        /// </summary>
        /// <typeparam name="TConstructor">Type to validate</typeparam>
        /// <returns>An instance of <see cref="ArgumentsValidator"/></returns>
        public static ArgumentsValidator GetConstructorArgumentsValidator<TConstructor>()
            where TConstructor : class
        {
            var constructor = GetSinglePublicConstructor<TConstructor>();

            return GetMethodArgumentsValidator(null, constructor, null);
        }

        /// <summary>
        /// Creates an instance of <see cref="ArgumentsValidator"/> for the constructor of type <typeparamref name="TConstructor"/>.
        /// </summary>
        /// <param name="constructorArguments">Optional arguments used for testing</param>
        /// <typeparam name="TConstructor">Type to validate</typeparam>
        /// <returns>An instance of <see cref="ArgumentsValidator"/></returns>
        public static ArgumentsValidator GetConstructorArgumentsValidator<TConstructor>(object[] constructorArguments)
            where TConstructor : class
        {
            var constructor = GetSinglePublicConstructor<TConstructor>();

            return GetConstructorArgumentsValidator(constructor, constructorArguments);
        }

        /// <summary>
        /// Creates an instance of <see cref="ArgumentsValidator"/> for the specified ConstructorInfo
        /// </summary>
        /// <param name="constructor">Constructor to test</param>
        /// <param name="constructorArguments">Optional arguments used for testing</param>
        /// <returns>An instance of <see cref="ArgumentsValidator"/></returns>
        public static ArgumentsValidator GetConstructorArgumentsValidator(ConstructorInfo constructor, object[] constructorArguments)
        {
            return GetMethodArgumentsValidator(null, constructor, constructorArguments);
        }

        private static ConstructorInfo GetSinglePublicConstructor<TConstructor>()
            where TConstructor : class
        {
            var constructors = typeof(TConstructor).GetConstructors();

            if (constructors.Length == 0)
            {
                throw new ArgumentException($"Type {typeof(TConstructor).FullName} has no public constructors",
                                            nameof(TConstructor));
            }

            if (constructors.Length > 1)
            {
                throw new ArgumentException($"Type {typeof(TConstructor).FullName} has more than one public constructors",
                                            nameof(TConstructor));
            }

            var constructor = constructors.Single();

            return constructor;
        }

        /// <summary>
        /// Creates an instance of <see cref="ArgumentsValidator"/> for the method <paramref name="methodName"/> of <paramref name="targetObject"/>
        /// </summary>
        /// <param name="targetObject">Object to test</param>
        /// <param name="methodName">Name of the method to test</param>
        /// <param name="methodArguments">Optional arguments used for testing</param>
        /// <param name="objectCreators">Optional object creators used to create types during testing</param>
        /// <returns>An instance of <see cref="ArgumentsValidator"/></returns>
        public static ArgumentsValidator GetMethodArgumentsValidator(object targetObject, string methodName, object[] methodArguments = null, IEnumerable<IObjectCreator> objectCreators = null)
        {
            if (targetObject == null)
            {
                throw new ArgumentNullException(nameof(targetObject));
            }

            var creators = objectCreators?.ToArray();

            ValidateObjectCreators(creators);

            var method = targetObject.GetType().GetMethods().FirstOrDefault(x => x.Name == methodName);
            if (method == null)
            {
                throw new ArgumentException($"Cannot find method with name '{methodName}' on type '{targetObject.GetType().FullName}'", nameof(methodName));
            }

            return new ArgumentsValidator(targetObject, method, methodArguments,
                                          creators ?? DefaultCreators);
        }

        /// <summary>
        /// Creates an instance of <see cref="ArgumentsValidator"/> for the method <paramref name="method"/>
        /// </summary>
        /// <param name="targetObject">Object to test. Can be null if testing a static method.</param>
        /// <param name="method">Method to test</param>
        /// <param name="methodArguments">Optional arguments used for testing</param>
        /// <param name="objectCreators">Optional object creators used to create types during testing</param>
        /// <returns>An instance of <see cref="ArgumentsValidator"/></returns>
        public static ArgumentsValidator GetMethodArgumentsValidator(object targetObject, MethodBase method, object[] methodArguments = null, IEnumerable<IObjectCreator> objectCreators = null)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            
            if (targetObject != null && method.DeclaringType != targetObject.GetType())
            {
                throw new MissingMethodException(targetObject.GetType().FullName, method.Name);
            }

            var creators = objectCreators?.ToArray();

            ValidateObjectCreators(creators);
            
            return new ArgumentsValidator(targetObject, method, methodArguments,
                                          creators ?? DefaultCreators);
        }

        private static void ValidateObjectCreators(IObjectCreator[] objectCreators)
        {
            if (objectCreators is { Length: 0 })
            {
                throw new ArgumentException("Value cannot be an empty collection.",
                                            nameof(objectCreators));
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="ArgumentsValidator"/> for the expression.
        /// <remarks>This is currently experimental</remarks>
        /// </summary>
        /// <typeparam name="T">Type of the object to test</typeparam>
        /// <param name="targetObject">Object to test</param>
        /// <param name="objectCreators">Optional object creators used to create types during testing</param>
        /// <returns></returns>
        public static ExpressionArgumentsValidator<T> GetExpressionArgumentsValidator<T>(T targetObject, IEnumerable<IObjectCreator> objectCreators = null)
            where T : class
        {
            return new ExpressionArgumentsValidator<T>(targetObject, objectCreators);
        }
    }
}