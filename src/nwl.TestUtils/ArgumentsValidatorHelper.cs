using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using nwl.TestingUtilities.ObjectCreators;

namespace nwl.TestingUtilities
{
    public static class ArgumentsValidatorHelper
    {
        public static IObjectCreator[] DefaultCreators { get; } = new IObjectCreator[]
                                                                  {
                                                                      new ListCreator(),
                                                                      new ArrayCreator(),
                                                                      new GenericDictionaryCreator(),
                                                                      new GenericIEnumerableCreator(),
                                                                      new GenericIListCreator(),
                                                                      new GenericICollectionCreator(),
                                                                      new MoqInterfaceCreator(),
                                                                      new ValueTypeCreator()
                                                                  };

        public static ArgumentsValidator GetConstructorArgumentsValidator<TConstructor>()
            where TConstructor : class
        {
            var constructor = GetSinglePublicConstructor<TConstructor>();

            return GetMethodArgumentsValidator(null, constructor, null);
        }

        public static ArgumentsValidator GetConstructorArgumentsValidator<TConstructor>(object[] constructorArguments)
            where TConstructor : class
        {
            var constructor = GetSinglePublicConstructor<TConstructor>();

            return GetConstructorArgumentsValidator(constructor, constructorArguments);
        }

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

            return new ArgumentsValidator(targetObject, method, creators ?? DefaultCreators, methodArguments);
        }
        
        public static ArgumentsValidator GetMethodArgumentsValidator(object targetObject, MethodBase method, object[] methodArguments = null, IEnumerable<IObjectCreator> objectCreators = null)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var creators = objectCreators?.ToArray();

            ValidateObjectCreators(creators);
            
            return new ArgumentsValidator(targetObject, method, creators ?? DefaultCreators, methodArguments);
        }

        private static void ValidateObjectCreators(IObjectCreator[] objectCreators)
        {
            if (objectCreators is { Length: 0 })
            {
                throw new ArgumentException("Value cannot be an empty collection.",
                                            nameof(objectCreators));
            }
        }
    }
}