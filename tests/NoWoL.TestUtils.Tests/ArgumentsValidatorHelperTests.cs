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
            var validator = ParametersValidatorHelper.GetConstructorParametersValidator<TestClassWithOnePublicConstructor>();
            validator.SetupParameter("param1",
                                     ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetConstructorArgumentsValidatorThrowsIfMultiplePublicConstructor()
        {
            var ex = Assert.Throws<ArgumentException>(() => ParametersValidatorHelper.GetConstructorParametersValidator<TestClassWithTwoPublicConstructor>());

            Assert.Equal($"Type {typeof(TestClassWithTwoPublicConstructor).FullName} has more than one public constructors (Parameter 'TConstructor')",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetConstructorArgumentsValidatorThrowsIfNoPublicConstructor()
        {
            var ex = Assert.Throws<ArgumentException>(() => ParametersValidatorHelper.GetConstructorParametersValidator<TestClassWithNoPublicConstructor>());

            Assert.Equal($"Type {typeof(TestClassWithNoPublicConstructor).FullName} has no public constructors (Parameter 'TConstructor')",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetConstructorArgumentsValidatorWithDefaultArgumentsCreatesAValidValidator()
        {
            var validator = ParametersValidatorHelper.GetConstructorParametersValidator<TestClassWithOnePublicConstructor>(new object[] { "Freddie" });
            validator.SetupParameter("param1",
                                     ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetConstructorArgumentsValidatorWithSpecifiedConstructorCreatesAValidValidator()
        {
            var ctorInfo = typeof(TestClassWithOnePublicConstructor).GetConstructors().Single();
            var validator = ParametersValidatorHelper.GetConstructorParametersValidator(ctorInfo,
                                                                                        new object[] { "Freddie" });
            validator.SetupParameter("param1",
                                     ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorCreatesAValidValidator_MethodInfoVariant()
        {
            var obj = new SimpleTestClass();
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithStringValidation));

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   method);
            validator.SetupParameter("param1",
                                     ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorCreatesAValidValidator_StringVariant()
        {
            var obj = new SimpleTestClass();

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   nameof(SimpleTestClass.MethodWithStringValidation));
            validator.SetupParameter("param1",
                                     ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorThrowsIfMethodCannotBeFound_StringVariant()
        {
            var obj = new ComplexTestClass();

            var ex = Assert.Throws<ArgumentException>(() => ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                                                   "unknown method"));

            Assert.Equal($"Cannot find method with name 'unknown method' on type '{typeof(ComplexTestClass).FullName}' (Parameter 'methodName')",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorThrowsIfMethodIsNull_MethodInfoVariant()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => ParametersValidatorHelper.GetMethodParametersValidator(null,
                                                                                                                       (MethodBase)null));

            Assert.Equal("method",
                         ex.ParamName);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorThrowsIfObjectCreatorIsEmpty_MethodVariant()
        {
            var obj = new SimpleTestClass();
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithStringValidation));

            var ex = Assert.Throws<ArgumentException>(() => ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                                                   method,
                                                                                                                   objectCreators: Array.Empty<IObjectCreator>()));

            Assert.Equal("Value cannot be an empty collection. (Parameter 'objectCreators')",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorThrowsIfObjectCreatorIsEmpty_StringVariant()
        {
            var obj = new SimpleTestClass();

            var ex = Assert.Throws<ArgumentException>(() => ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                                                   nameof(SimpleTestClass.MethodWithStringValidation),
                                                                                                                   objectCreators: Array.Empty<IObjectCreator>()));

            Assert.Equal("Value cannot be an empty collection. (Parameter 'objectCreators')",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorThrowsIfTargetObjectIsNull_StringVariant()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => ParametersValidatorHelper.GetMethodParametersValidator(null,
                                                                                                                       "unknown method"));

            Assert.Equal("targetObject",
                         ex.ParamName);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorThrowsIfTheMethodIsNotAvailableOnTargetObject()
        {
            var obj = new SimpleTestClass();
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));

            var ex = Assert.Throws<MissingMethodException>(() => ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                                                        method));
            Assert.Equal("Method 'NoWoL.TestingUtilities.Tests.SimpleTestClass.SomeMethod' not found.",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorUsesSpecifiedObjectCreators_MethodInfoVariant()
        {
            var obj = new SimpleTestClass();
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithConcreteClassWithRules));

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   method,
                                                                                   objectCreators: new IObjectCreator[] { new ComplexTestClassObjectCreator() });
            validator.SetupParameter("paramO",
                                     ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetMethodArgumentsValidatorUsesSpecifiedObjectCreators_StringVariant()
        {
            var obj = new SimpleTestClass();

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   nameof(SimpleTestClass.MethodWithConcreteClassWithRules),
                                                                                   objectCreators: new IObjectCreator[] { new ComplexTestClassObjectCreator() });
            validator.SetupParameter("paramO",
                                     ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateExpression()
        {
            var obj = new ComplexTestClass();

            var validator = ParametersValidatorHelper.GetExpressionParametersValidator(obj);
            validator.Setup(x => x.SomeMethod(validator.For<string>(ExpectedExceptionRules.NotNull,
                                                                    ExpectedExceptionRules.NotEmptyOrWhiteSpace),
                                              validator.For<ISomeInterface>(ExpectedExceptionRules.NotNull),
                                              validator.For<List<ISomeInterface>>(ExpectedExceptionRules.NotNull,
                                                                                  ExpectedExceptionRules.NotEmpty),
                                              validator.For<ISomeInterface[]>(ExpectedExceptionRules.NotNull,
                                                                              ExpectedExceptionRules.NotEmpty),
                                              validator.For<IEnumerable<ISomeInterface>>(ExpectedExceptionRules.NotNull,
                                                                                         ExpectedExceptionRules.NotEmpty)))
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ValidateExpressionAsync()
        {
            var obj = new SimpleTestClass();

            var validator = ParametersValidatorHelper.GetExpressionParametersValidator(obj);
            await validator.Setup(x => x.MethodWithStringValidationAsync(validator.For<string>(ExpectedExceptionRules.NotNull,
                                                                                               ExpectedExceptionRules.NotEmpty,
                                                                                               ExpectedExceptionRules.NotEmptyOrWhiteSpace)))
                           .ValidateAsync().ConfigureAwait(false);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateExpressionUsesTheSpecifiedObjectCreators()
        {
            var obj = new SimpleTestClass();

            var validator = ParametersValidatorHelper.GetExpressionParametersValidator(obj,
                                                                                       new[] { new ComplexTestClassObjectCreator() });
            validator.Setup(x => x.MethodWithConcreteClass(validator.For<ComplexTestClass>(ExpectedExceptionRules.None)))
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateMethodWithOutParameters()
        {
            var obj = new TestClassWithOutParameter();

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   nameof(TestClassWithOutParameter.MethodToTest));
            validator.SetupParameter("paramO",
                                     ExpectedExceptionRules.NotNull)
                     .SetupParameter("stringParam",
                                     ExpectedExceptionRules.None)
                     .SetupParameter("interfaceParam",
                                     ExpectedExceptionRules.None)
                     .SetupParameter("listParam",
                                     ExpectedExceptionRules.None)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateMethodWithRefParameters()
        {
            var obj = new TestClassWithRefParameter();

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   nameof(TestClassWithRefParameter.MethodToTest));
            validator.SetupParameter("paramO",
                                     ExpectedExceptionRules.NotNull)
                     .SetupParameter("stringParam",
                                     ExpectedExceptionRules.NotNull)
                     .SetupParameter("interfaceParam",
                                     ExpectedExceptionRules.None)
                     .SetupParameter("listParam",
                                     ExpectedExceptionRules.None)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetParameterValueConfiguresTheValueUsedForTesting()
        {
            var obj = new SimpleTestClass();
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithConcreteClassWithRules));

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   method);
            validator.SetupAll(ParametersValidator.DefaultRules)
                     .SetParameterValue("paramO", new ComplexTestClass())
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetParameterValueCannotOverrideValuesFromMethodParameters()
        {
            var obj = new SimpleTestClass();
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithConcreteClassWithRules));

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(obj,
                                                                                   method,
                                                                                   new object[]
                                                                                   {
                                                                                       new ComplexTestClass()
                                                                                   });

            Assert.Throws<UseMethodParametersValuesInsteadException>(() => validator.SetupAll(ParametersValidator.DefaultRules)
                                                                                    .SetParameterValue("paramO",
                                                                                                       null)
                                                                                    .Validate());
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateInputParametersForSetParameterValue()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethodWith2Parameters));
            var validator = ParametersValidatorHelper.GetMethodParametersValidator(new ParametersValidator(new ComplexTestClass(),
                                                                                                           method,
                                                                                                           null,
                                                                                                           ParametersValidatorHelper.DefaultCreators),
                                                                                   nameof(ParametersValidator.SetParameterValue));

            var sut = ParametersValidatorHelper.GetMethodParametersValidator(validator,
                                                                             nameof(ParametersValidator.SetParameterValue));

            sut.SetupAll(ParametersValidator.DefaultRules)
               .SetParameterValue("paramName", "paramName")
               .SetParameterValue("value", 3)
               .UpdateParameter("value",
                                ExpectedExceptionRules.None).Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetParameterValueThrowsIfParamNameCannotBeFound()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethodWith2Parameters));
            var validator = ParametersValidatorHelper.GetMethodParametersValidator(new ParametersValidator(new ComplexTestClass(),
                                                                                                           method,
                                                                                                           null,
                                                                                                           ParametersValidatorHelper.DefaultCreators),
                                                                                   nameof(ParametersValidator.SetParameterValue));

            var sut = ParametersValidatorHelper.GetMethodParametersValidator(validator,
                                                                             nameof(ParametersValidator.SetParameterValue));

            var ex = Assert.Throws<ArgumentException>(() => sut.SetupAll(ParametersValidator.DefaultRules)
                                                               .SetParameterValue("SomeParam",
                                                                                  3));
            Assert.Equal("paramName",
                         ex.ParamName);
            Assert.Equal("Parameter 'SomeParam' does not exists on the method. (Parameter 'paramName')",
                         ex.Message);
        }
    }
}