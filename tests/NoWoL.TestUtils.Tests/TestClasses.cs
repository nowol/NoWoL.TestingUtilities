using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

#pragma warning disable IDE0060 // Remove unused parameter

namespace NoWoL.TestingUtilities.Tests
{
    [ExcludeFromCodeCoverage]
    public class MethodsHolder
    {
        public static MethodInfo GetMethodInfoWithNoParameters()
        {
            return typeof(MethodsHolder).GetMethod(nameof(MethodWithNoParameters))!;
        }

        public static ParameterInfo GetStringParameterInfo()
        {
            return typeof(MethodsHolder).GetMethod(nameof(MethodWithOneString))!.GetParameters().Single();
        }

        public static ParameterInfo GetStringArrayParameterInfo()
        {
            return typeof(MethodsHolder).GetMethod(nameof(MethodWithOneStringArray))!.GetParameters().Single();
        }

        public static ParameterInfo GetStringListParameterInfo()
        {
            return typeof(MethodsHolder).GetMethod(nameof(MethodWithOneStringList))!.GetParameters().Single();
        }

        public static ParameterInfo GetStringIEnumerableParameterInfo()
        {
            return typeof(MethodsHolder).GetMethod(nameof(MethodWithOneStringIEnumerable))!.GetParameters().Single();
        }

        public static ParameterInfo GetStringICollectionParameterInfo()
        {
            return typeof(MethodsHolder).GetMethod(nameof(MethodWithOneStringICollection))!.GetParameters().Single();
        }

        public static ParameterInfo GetStringIListParameterInfo()
        {
            return typeof(MethodsHolder).GetMethod(nameof(MethodWithOneStringIList))!.GetParameters().Single();
        }

        public static ParameterInfo GetIntParameterInfo()
        {
            return typeof(MethodsHolder).GetMethod(nameof(MethodWithOneInteger))!.GetParameters().Single();
        }

        public static ParameterInfo GetActionParameterInfo()
        {
            return typeof(MethodsHolder).GetMethod(nameof(MethodWithOneAction))!.GetParameters().Single();
        }

        public void MethodWithNoParameters()
        { }

        public void MethodWithOneInteger(int param)
        { }

        public void MethodWithOneString(string param)
        { }

        public void MethodWithOneStringArray(string[] param)
        { }

        public void MethodWithOneStringList(List<string> param)
        { }

        public void MethodWithOneStringIEnumerable(IEnumerable<string> param)
        { }

        public void MethodWithOneStringICollection(ICollection<string> param)
        { }

        public void MethodWithOneStringIList(IList<string> param)
        { }

        public void MethodWithOneAction(Action<string> param)
        { }
    }

    public interface ISomeInterface
    { }

    [ExcludeFromCodeCoverage]
    public class ComplexTestClassObjectCreator : IObjectCreator
    {
        public bool CanHandle(Type type)
        {
            return type == typeof(ComplexTestClass);
        }

        public object Create(Type type, ICollection<IObjectCreator> objectCreators)
        {
            return new ComplexTestClass();
        }
    }

    [ExcludeFromCodeCoverage]
    public class SimpleTestClass
    {
        public string MethodWithStringValidation(string param1)
        {
            if (param1 == null)
            {
                throw new ArgumentNullException(nameof(param1));
            }

            if (string.IsNullOrEmpty(param1))
            {
                throw new ArgumentException("Cannot be empty",
                                            nameof(param1));
            }

            if (string.IsNullOrWhiteSpace(param1))
            {
                throw new ArgumentException("Cannot be empty or whitespace",
                                            nameof(param1));
            }

            return null;
        }

        public async Task<string> MethodWithStringValidationAsync(string param1)
        {
            if (param1 == null)
            {
                throw new ArgumentNullException(nameof(param1));
            }

            if (string.IsNullOrEmpty(param1))
            {
                throw new ArgumentException("Cannot be empty",
                                            nameof(param1));
            }

            if (string.IsNullOrWhiteSpace(param1))
            {
                throw new ArgumentException("Cannot be empty or whitespace",
                                            nameof(param1));
            }

            await Task.Delay(1).ConfigureAwait(false);

            return null;
        }

        public async ValueTask<string> MethodWithStringValidationAsyncWithGenericValueTask(string param1)
        {
            if (param1 == null)
            {
                throw new ArgumentNullException(nameof(param1));
            }

            if (string.IsNullOrEmpty(param1))
            {
                throw new ArgumentException("Cannot be empty",
                                            nameof(param1));
            }

            if (string.IsNullOrWhiteSpace(param1))
            {
                throw new ArgumentException("Cannot be empty or whitespace",
                                            nameof(param1));
            }

            await Task.Delay(1).ConfigureAwait(false);

            return null;
        }

        public string MethodWithNoValidation(string paramW)
        {
            return null;
        }

        public Task MethodWithNoValidationAsync(string paramW)
        {
            return Task.CompletedTask;
        }

        public ValueTask MethodWithNoValidationAsyncWithNonGenericValueTask(string paramW)
        {
            return ValueTask.CompletedTask;
        }

        public string MethodWithConcreteClass(ComplexTestClass paramO)
        {
            return null;
        }

        public string MethodWithConcreteClassWithRules(ComplexTestClass paramO)
        {
            if (paramO == null)
            {
                throw new ArgumentNullException(nameof(paramO));
            }

            return null;
        }
    }

    [ExcludeFromCodeCoverage]
    public class TestClassWithOnePublicConstructor
    {
        public TestClassWithOnePublicConstructor(string param1)
        {
            if (param1 == null)
            {
                throw new ArgumentNullException(nameof(param1));
            }
        }
    }

    [ExcludeFromCodeCoverage]
    public class TestClassWithNoPublicConstructor
    {
        protected TestClassWithNoPublicConstructor(string param1)
        {
            if (param1 == null)
            {
                throw new ArgumentNullException(nameof(param1));
            }
        }
    }

    [ExcludeFromCodeCoverage]
    public class TestClassWithOutParameter
    {
        public string MethodToTest(string paramO, out string stringParam, out ISomeInterface interfaceParam, out List<int> listParam)
        {
            if (paramO == null)
            {
                throw new ArgumentNullException(nameof(paramO));
            }

            stringParam = null;
            interfaceParam = null;
            listParam = null;

            return "Some value";
        }
    }

    [ExcludeFromCodeCoverage]
    public class TestClassWithRefParameter
    {
        public string MethodToTest(string paramO, ref string stringParam, ref ISomeInterface interfaceParam, ref List<int> listParam)
        {
            if (paramO == null)
            {
                throw new ArgumentNullException(nameof(paramO));
            }

            if (stringParam == null)
            {
                throw new ArgumentNullException(nameof(stringParam));
            }

            return "Some value";
        }
    }

    [ExcludeFromCodeCoverage]
    public class TestClassWithTwoPublicConstructor
    {
        public TestClassWithTwoPublicConstructor(string param1)
        {
            if (param1 == null)
            {
                throw new ArgumentNullException(nameof(param1));
            }
        }

        public TestClassWithTwoPublicConstructor(string param1, int param2)
        {
            if (param1 == null)
            {
                throw new ArgumentNullException(nameof(param1));
            }
        }
    }

    [ExcludeFromCodeCoverage]
    public class ComplexTestClass
    {
        public string SomeMethod(string param1, ISomeInterface param2, List<ISomeInterface> param3, ISomeInterface[] param4, IEnumerable<ISomeInterface> param5)
        {
            if (param1 == null)
            {
                throw new ArgumentNullException(nameof(param1));
            }

            if (string.IsNullOrEmpty(param1))
            {
                throw new ArgumentException("Cannot be empty",
                                            nameof(param1));
            }

            if (string.IsNullOrWhiteSpace(param1))
            {
                throw new ArgumentException("Cannot be empty or whitespace",
                                            nameof(param1));
            }

            if (param2 == null)
            {
                throw new ArgumentNullException(nameof(param2));
            }

            if (param3 == null)
            {
                throw new ArgumentNullException(nameof(param3));
            }

            if (param3.Count == 0)
            {
                throw new ArgumentException("Cannot be empty",
                                            nameof(param3));
            }

            if (param4 == null)
            {
                throw new ArgumentNullException(nameof(param4));
            }

            if (param4.Length == 0)
            {
                throw new ArgumentException("Cannot be empty",
                                            nameof(param4));
            }

            if (param5 == null)
            {
                throw new ArgumentNullException(nameof(param5));
            }

            if (!param5.Any())
            {
                throw new ArgumentException("Cannot be empty",
                                            nameof(param5));
            }

            return null;
        }

        public string MethodForDefaultRules(string param1,
                                            ISomeInterface param2,
                                            List<ISomeInterface> param3,
                                            ISomeInterface[] param4,
                                            IEnumerable<ISomeInterface> param5,
                                            Dictionary<string, string> param6,
                                            int param7,
                                            ComplexTestClass param8)
        {
            if (param1 == null)
            {
                throw new ArgumentNullException(nameof(param1));
            }

            if (string.IsNullOrWhiteSpace(param1))
            {
                throw new ArgumentException("Cannot be empty or whitespace",
                                            nameof(param1));
            }

            if (param2 == null)
            {
                throw new ArgumentNullException(nameof(param2));
            }

            if (param3 == null)
            {
                throw new ArgumentNullException(nameof(param3));
            }

            if (param4 == null)
            {
                throw new ArgumentNullException(nameof(param4));
            }

            if (param5 == null)
            {
                throw new ArgumentNullException(nameof(param5));
            }

            if (param6 == null)
            {
                throw new ArgumentNullException(nameof(param6));
            }

            if (param8 == null)
            {
                throw new ArgumentNullException(nameof(param8));
            }

            return null;
        }
    }
}