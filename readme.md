
# NoWoL.TestingUtilities

![Build Status](https://dev.azure.com/nowol/DesertOctopus/_apis/build/status/nowol.nwl.TestingUtilities?branchName=main)
[![Version](https://img.shields.io/nuget/vpre/NoWoL.TestingUtilities.svg)](https://www.nuget.org/packages/NoWoL.TestingUtilities)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nowol_NoWoL.TestingUtilities&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=nowol_NoWoL.TestingUtilities)
![Sonar Coverage](https://img.shields.io/sonar/coverage/nowol_NoWoL.TestingUtilities/main?server=https%3A%2F%2Fsonarcloud.io)
![Sonar Tests](https://img.shields.io/sonar/tests/nowol_NoWoL.TestingUtilities/main?compact_message&server=https%3A%2F%2Fsonarcloud.io)

NoWoL.TestingUtilities is a collection of testing utilities for C# development. Presently it contains helper methods to help test parameters validation for constructor and public methods.

## Installation

You can install the `NoWoL.TestingUtilities` packages using your favorite Nuget package manager.

## Usage

Before you can test the input parameters of a method or constructor you need to acquire a `ParametersValidator`. The library provides a helper class called `ParametersValidatorHelper` to help with the validator creation. Once you have your validator you will need to configure every parameters using the `SetupParameter` method and then call `Validate` to test the parameters.

```csharp
// the sample code below will use the following class definition 
public class TestClass
{
    public TestClass(string param1, IMyInterface param2)
    {
        if (param1 == null) throw new ArgumentNullException(nameof(param1));
        if (String.IsNullOrWhiteSpace(param1)) throw new ArgumentException("Cannot be null, empty or whitespace", nameof(param1));

        if (param2 == null) throw new ArgumentNullException(nameof(param2));
    }

    public void MyMethod(string param1, List<int> param2)
    {
        if (param2 == null) throw new ArgumentNullException(nameof(param2));
        if (param2.Count == 0) throw new ArgumentException("Cannot be empty", nameof(param2));
    }
}

// Validates the constructor's parameters
var validator = ParametersValidatorHelper.GetConstructorParametersValidator<TestClass>();
validator.SetupParameter("param1", ExpectedExceptionRules.NotNull, ExpectedExceptionRules.ExpectedNotEmptyOrWhiteSpaceException) // validates that the string is not null, empty or only white spaces
         .SetupParameter("param2", ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmpty) // validates that the list is not null and not empty
         .Validate();

// Validates the method's parameters
var obj = new TestClass();
var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj, nameof(TestClass.MyMethod));
validator.SetupParameter("param1", ExpectedExceptionRules.None) // validates that no exception are thrown for the parameter
         .SetupParameter("param2", ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmpty)
         .Validate();
```

It is also possible to test `async/await` code using the `ValidateAsync` method.

### Expressions

You can use a LINQ expression to avoid using magic strings for the name of the parameters. The helper below will automatically fill in the parameters' names in the order they are used in the expression. Using `validator.For` is currently required to correctly wire up the validator since `ConstantExpression` are not yet supported.

```csharp
var obj = new TestClass();
var validator = ParametersValidatorHelper.GetExpressionParametersValidator(obj);
validator.Setup(x => x.MyMethod(validator.For<string>(ExpectedExceptionRules.None),
                                validator.For<List<int>>(ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmpty)))
         .Validate();
```

### Automatic Setup

Configuring every parameters manually can be quite a chore. You can use the `Setup` method to automatically configure the parameters' rules of a method using predefined rules.

```csharp
var obj = new TestClass();
var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj, nameof(TestClass.MyMethod));
validator.SetupAll(ParametersValidator.DefaultRules) // The default rules are the predefined validation rules for the most common validations. You can define you own rules if the defaults do not work for you.
         .UpdateParameter("param1", ExpectedExceptionRules.NotValue("Freddie")) // You can call SetupAll to configure every parameters with the default rules and then use UpdateParameter to update any parameters which the default rules are not applicable
         .Validate();
```

### Type creation

The library needs to know how to create the different object to correctly call the methods under test. The most common types are handled by default however you may need to create a type that is not handled by default. To do so, you will need to create an instance of `IObjectCreator` to handle the type creation.

```csharp
public class TestClassObjectCreator : IObjectCreator
{
    public bool CanHandle(Type type)
    {
        if (type == null)  throw new ArgumentNullException(nameof(type));

        return type == typeof(TestClass);
    }

    public object Create(Type type, ICollection<IObjectCreator> objectCreators)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        if (objectCreators == null) throw new ArgumentNullException(nameof(objectCreators));

        if (CanHandle(type))
        {
            return new TestClass("Text", CreatorHelpers.CreateItemFromType(typeof(IMyInterface), objectCreators)); // interfaces will be handled by the Moq object creator
        }

        throw new NotSupportedException("Expecting a List<> type however received " + type.FullName);
    }
}

// You will need to create a new collection of object creators for your validator:
var creators = new List<IObjectCreator>();
creators.Add(new TestClassObjectCreator()); // you should add your object creators before the default ones
creators.AddRange(ParametersValidatorHelper.DefaultCreators); // add the default creators
var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj, 
                                                                       nameof(AnotherClass.AnotherMethod), 
                                                                       objectCreators: creators);
```

To avoid creating too many one-off object creators you can simply create the test values yourself and pass them to the validator.

```csharp
var values = new object[]
             {
                 "firstvalue",
                 new List<int> { 3 },
                 ...
             };
var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj, 
                                                                       nameof(AnotherClass.AnotherMethod), 
                                                                       methodParameters: values);
...
```

### Exception Validation Rules

The library comes with the most common validation rules for no exception, not empty, not empty or white-space, not null and specific invalid value. The `ExpectedExceptionRules` class holds instances of these rules.

You can create your own validation rule by inheriting from `IExpectedException`. This interface provides 2 main methods: `Evaluate` which is used to analyze an exception that was thrown (if any) and `GetInvalidParameterValue` which is used to generate and invalid value used as input for the method under test.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](LICENSE)
