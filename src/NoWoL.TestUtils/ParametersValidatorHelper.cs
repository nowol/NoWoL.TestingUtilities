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
    /// Helper class to create an instance of <see cref="ParametersValidator"/>
    /// </summary>
    public static class ParametersValidatorHelper
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
        /// Creates an instance of <see cref="ParametersValidator"/> for the constructor of type <typeparamref name="TConstructor"/>.
        /// </summary>
        /// <typeparam name="TConstructor">Type to validate</typeparam>
        /// <returns>An instance of <see cref="ParametersValidator"/></returns>
        public static ParametersValidator GetConstructorParametersValidator<TConstructor>()
            where TConstructor : class
        {
            var constructor = GetSinglePublicConstructor<TConstructor>();

            return GetMethodParametersValidator(null, constructor, null);
        }

        /// <summary>
        /// Creates an instance of <see cref="ParametersValidator"/> for the constructor of type <typeparamref name="TConstructor"/>.
        /// </summary>
        /// <param name="constructorParameters">Optional values used for testing as arguments to the constructor</param>
        /// <typeparam name="TConstructor">Type to validate</typeparam>
        /// <returns>An instance of <see cref="ParametersValidator"/></returns>
        public static ParametersValidator GetConstructorParametersValidator<TConstructor>(object[] constructorParameters)
            where TConstructor : class
        {
            var constructor = GetSinglePublicConstructor<TConstructor>();

            return GetConstructorParametersValidator(constructor, constructorParameters);
        }

        /// <summary>
        /// Creates an instance of <see cref="ParametersValidator"/> for the specified ConstructorInfo
        /// </summary>
        /// <param name="constructor">Constructor to test</param>
        /// <param name="constructorParameters">Optional values used for testing as arguments to the constructor</param>
        /// <returns>An instance of <see cref="ParametersValidator"/></returns>
        public static ParametersValidator GetConstructorParametersValidator(ConstructorInfo constructor, object[] constructorParameters)
        {
            return GetMethodParametersValidator(null, constructor, constructorParameters);
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
        /// Creates an instance of <see cref="ParametersValidator"/> for the method <paramref name="methodName"/> of <paramref name="targetObject"/>
        /// </summary>
        /// <param name="targetObject">Object to test</param>
        /// <param name="methodName">Name of the method to test</param>
        /// <param name="methodParameters">Optional values used for testing as arguments for the method</param>
        /// <param name="objectCreators">Optional object creators used to create types during testing</param>
        /// <returns>An instance of <see cref="ParametersValidator"/></returns>
        public static ParametersValidator GetMethodParametersValidator(object targetObject, string methodName, object[] methodParameters = null, IEnumerable<IObjectCreator> objectCreators = null)
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

            return new ParametersValidator(targetObject, method, methodParameters,
                                          creators ?? DefaultCreators);
        }

        /// <summary>
        /// Creates an instance of <see cref="ParametersValidator"/> for the method <paramref name="method"/>
        /// </summary>
        /// <param name="targetObject">Object to test. Can be null if testing a static method.</param>
        /// <param name="method">Method to test</param>
        /// <param name="methodParameters">Optional values used for testing as arguments for the method</param>
        /// <param name="objectCreators">Optional object creators used to create types during testing</param>
        /// <returns>An instance of <see cref="ParametersValidator"/></returns>
        public static ParametersValidator GetMethodParametersValidator(object targetObject, MethodBase method, object[] methodParameters = null, IEnumerable<IObjectCreator> objectCreators = null)
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
            
            return new ParametersValidator(targetObject, method, methodParameters,
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
        /// Creates an instance of <see cref="ParametersValidator"/> for the expression.
        /// <remarks>This is currently experimental</remarks>
        /// </summary>
        /// <typeparam name="T">Type of the object to test</typeparam>
        /// <param name="targetObject">Object to test</param>
        /// <param name="objectCreators">Optional object creators used to create types during testing</param>
        /// <returns></returns>
        public static ExpressionParametersValidator<T> GetExpressionParametersValidator<T>(T targetObject, IEnumerable<IObjectCreator> objectCreators = null)
            where T : class
        {
            return new ExpressionParametersValidator<T>(targetObject, objectCreators);
        }
    }
}