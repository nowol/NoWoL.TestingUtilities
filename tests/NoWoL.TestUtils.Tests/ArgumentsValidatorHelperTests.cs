using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NoWoL.TestingUtilities.ExpectedExceptions;
using Xunit;

#pragma warning disable CA1707 // Identifiers should not contain underscores

namespace NoWoL.TestingUtilities.Tests
{
    public class ArgumentsValidatorHelperTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetConstructorArgumentsValidatorCreatesAValidValidator()
        {
            var validator = ArgumentsValidatorHelper.GetConstructorArgumentsValidator<TestClassWithOnePublicConstructor>();
            validator.SetupParameter("param1", ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetConstructorArgumentsValidatorWithDefaultArgumentsCreatesAValidValidator()
        {
            var validator = ArgumentsValidatorHelper.GetConstructorArgumentsValidator<TestClassWithOnePublicConstructor>(new object[] { "Freddie" });
            validator.SetupParameter("param1", ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetConstructorArgumentsValidatorWithSpecifiedConstructorCreatesAValidValidator()
        {
            var ctorInfo = typeof(TestClassWithOnePublicConstructor).GetConstructors().Single();
            var validator = ArgumentsValidatorHelper.GetConstructorArgumentsValidator(ctorInfo, new object[] { "Freddie" });
            validator.SetupParameter("param1", ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetConstructorArgumentsValidatorThrowsIfNoPublicConstructor()
        {
            var ex = Assert.Throws<ArgumentException>(() => ArgumentsValidatorHelper.GetConstructorArgumentsValidator<TestClassWithNoPublicConstructor>());

            Assert.Equal($"Type {typeof(TestClassWithNoPublicConstructor).FullName} has no public constructors (Parameter 'TConstructor')",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetConstructorArgumentsValidatorThrowsIfMultiplePublicConstructor()
        {
            var ex = Assert.Throws<ArgumentException>(() => ArgumentsValidatorHelper.GetConstructorArgumentsValidator<TestClassWithTwoPublicConstructor>());

            Assert.Equal($"Type {typeof(TestClassWithTwoPublicConstructor).FullName} has more than one public constructors (Parameter 'TConstructor')",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorThrowsIfObjectCreatorIsEmpty_StringVariant()
        {
            var obj = new SimpleTestClass();

            var ex = Assert.Throws<ArgumentException>(() => ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, nameof(SimpleTestClass.MethodWithStringValidation), objectCreators: Array.Empty<IObjectCreator>()));

            Assert.Equal("Value cannot be an empty collection. (Parameter 'objectCreators')",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorThrowsIfTargetObjectIsNull_StringVariant()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => ArgumentsValidatorHelper.GetMethodArgumentsValidator(null, "unknown method"));

            Assert.Equal("targetObject",
                         ex.ParamName);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorThrowsIfMethodCannotBeFound_StringVariant()
        {
            var obj = new ComplexTestClass();

            var ex = Assert.Throws<ArgumentException>(() => ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, "unknown method"));

            Assert.Equal($"Cannot find method with name 'unknown method' on type '{typeof(ComplexTestClass).FullName}' (Parameter 'methodName')",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorCreatesAValidValidator_StringVariant()
        {
            var obj = new SimpleTestClass();

            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, nameof(SimpleTestClass.MethodWithStringValidation));
            validator.SetupParameter("param1", ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorUsesSpecifiedObjectCreators_StringVariant()
        {
            var obj = new SimpleTestClass();

            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, nameof(SimpleTestClass.MethodWithConcreteClassWithRules), objectCreators: new IObjectCreator[] { new ComplexTestClassObjectCreator() });
            validator.SetupParameter("paramO", ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorThrowsIfObjectCreatorIsEmpty_MethodVariant()
        {
            var obj = new SimpleTestClass();
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithStringValidation));

            var ex = Assert.Throws<ArgumentException>(() => ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, method, objectCreators: Array.Empty<IObjectCreator>()));

            Assert.Equal("Value cannot be an empty collection. (Parameter 'objectCreators')",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorThrowsIfMethodIsNull_MethodInfoVariant()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => ArgumentsValidatorHelper.GetMethodArgumentsValidator(null, (MethodBase)null));

            Assert.Equal("method",
                         ex.ParamName);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorCreatesAValidValidator_MethodInfoVariant()
        {
            var obj = new SimpleTestClass();
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithStringValidation));

            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, method);
            validator.SetupParameter("param1", ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorUsesSpecifiedObjectCreators_MethodInfoVariant()
        {
            var obj = new SimpleTestClass();
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithConcreteClassWithRules));

            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, method, objectCreators: new IObjectCreator[] { new ComplexTestClassObjectCreator() });
            validator.SetupParameter("paramO", ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorThrowsIfTheMethodIsNotAvailableOnTargetObject()
        {
            var obj = new SimpleTestClass();
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));

            var ex = Assert.Throws<InvalidOperationException>(() => ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, method));
            Assert.Equal("The specified method 'SomeMethod' is not available on the targeted object (NoWoL.TestingUtilities.Tests.SimpleTestClass).",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateMethodWithOutParameters()
        {
            var obj = new TestClassWithOutParameter();

            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, nameof(TestClassWithOutParameter.MethodToTest));
            validator.SetupParameter("paramO", ExpectedExceptionRules.NotNull)
                     .SetupParameter("stringParam", ExpectedExceptionRules.None)
                     .SetupParameter("interfaceParam", ExpectedExceptionRules.None)
                     .SetupParameter("listParam", ExpectedExceptionRules.None)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateMethodWithRefParameters()
        {
            var obj = new TestClassWithRefParameter();

            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(obj, nameof(TestClassWithRefParameter.MethodToTest));
            validator.SetupParameter("paramO", ExpectedExceptionRules.NotNull)
                     .SetupParameter("stringParam", ExpectedExceptionRules.NotNull)
                     .SetupParameter("interfaceParam", ExpectedExceptionRules.None)
                     .SetupParameter("listParam", ExpectedExceptionRules.None)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateExpression()
        {
            var obj = new ComplexTestClass();

            var validator = ArgumentsValidatorHelper.GetExpressionArgumentsValidator(obj);
            validator.Setup(x => x.SomeMethod(validator.For<string>(ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmptyOrWhiteSpace),
                                              validator.For<ISomeInterface>(ExpectedExceptionRules.NotNull),
                                              validator.For<List<ISomeInterface>>(ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmpty),
                                              validator.For<ISomeInterface[]>(ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmpty),
                                              validator.For<IEnumerable<ISomeInterface>>(ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmpty)))
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateExpressionUsesTheSpecifiedObjectCreators()
        {
            var obj = new SimpleTestClass();

            var validator = ArgumentsValidatorHelper.GetExpressionArgumentsValidator(obj, objectCreators: new [] { new ComplexTestClassObjectCreator() });
            validator.Setup(x => x.MethodWithConcreteClass(validator.For<ComplexTestClass>(ExpectedExceptionRules.None)))
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ValidateExpressionAsync()
        {
            var obj = new SimpleTestClass();

            var validator = ArgumentsValidatorHelper.GetExpressionArgumentsValidator(obj);
            await validator.Setup(x => x.MethodWithStringValidationAsync(validator.For<string>(ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmpty, ExpectedExceptionRules.NotEmptyOrWhiteSpace)))
                           .ValidateAsync().ConfigureAwait(false);
        }
    }
}