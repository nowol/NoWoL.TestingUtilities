# nwl.TestingUtilities

NoWoL.TestingUtilities is a collection of testing utilities for C# development. Presently it contains helper methods to help test parameters validation for constructor and public methods.

## Installation

You can install the `NoWoL.TestingUtilities` packages using your favorite Nuget package manager.

## Usage

Before you can test the input arguments of a method or constructor you need to acquire an `ArgumentsValidator`. The library provides a helper class called `ArgumentsValidatorHelper` to help with the validator creation. Once you have your validator you will need to configure every parameters using the `SetupParameter` method and then call `Validate` to test the parameters.

> Note: `out/ref` parameters are not yet supported.

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
var validator = ArgumentsValidatorHelper.GetConstructorArgumentsValidator<TestClass>();
validator.SetupParameter("param1", ExpectedExceptionRules.NotNull, ExpectedExceptionRules.ExpectedNotEmptyOrWhiteSpaceException) // validates that the string is not null, empty or only white spaces
         .SetupParameter("param2", ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmpty) // validates that the list is not null and not empty
         .Validate();

// Validates the method's parameters
var obj = new TestClass();
var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, nameof(TestClass.MyMethod));
validator.SetupParameter("param1", ExpectedExceptionRules.None) // validates that no exception are thrown for the parameter
         .SetupParameter("param2", ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmpty)
         .Validate();
```

It is also possible to test `async/await` code using the `ValidateAsync` method.

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
            return new TestClass("Text", CreatorHelpers.CreateItemFromType(typeof(IMyInterface))); // interfaces will be handled by the Moq object creator
        }

        throw new NotSupportedException("Expecting a List<> type however received " + type.FullName);
    }
}

// You will need to create a new collection of object creators for your validator:
var creators = new List<IObjectCreator>();
creators.Add(new TestClassObjectCreator()); // you should add your object creators before the default ones
creators.AddRange(ArgumentsValidatorHelper.DefaultCreators); // add the default creators
var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, 
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
var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, 
                                                                     nameof(AnotherClass.AnotherMethod), 
                                                                     methodArguments: values);
...
```

### Exception Validation Rules

The library comes with the most common validation rules for no exception, not empty, not empty or white-space, not null and specific invalid value. The `ExpectedExceptionRules` class holds instances of these rules.

You can create your own validation rule by inheriting from `IExpectedException`. This class provides 2 main methods: `Evaluate` which is used to analyze an exception that was thrown (if any) and `GetInvalidParameterValue` which is used to generate and invalid value used as input for the method under test.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](LICENSE)