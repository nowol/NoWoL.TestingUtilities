using System;
using System.Threading.Tasks;
using NoWoL.TestingUtilities.ExpectedExceptions;
using Xunit;

namespace NoWoL.TestingUtilities.Tests
{
    public class ArgumentsValidatorTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void ConstructorThrowIfInputParametersAreInvalid()
        {
            object[] parameters = new object[]
                                  {
                                      new ComplexTestClass(),
                                      typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod)),
                                      ArgumentsValidatorHelper.DefaultCreators,
                                      null
                                  };
            var validator = ArgumentsValidatorHelper.GetConstructorArgumentsValidator<ArgumentsValidator>(parameters);

            validator.SetupParameter("targetObject", ExpectedExceptionRules.None)
                     .SetupParameter("method", ExpectedExceptionRules.NotNull)
                     .SetupParameter("objectCreators", ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmpty)
                     .SetupParameter("methodArguments", ExpectedExceptionRules.None)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ConstructorThrowIfMethodHasNoParameters()
        {
            var ex = Assert.Throws<ArgumentException>(() => new ArgumentsValidator(null,
                                                                                   MethodsHolder.GetMethodInfoWithNoParameters(),
                                                                                   ArgumentsValidatorHelper.DefaultCreators,
                                                                                   null));
            Assert.Equal("method",
                         ex.ParamName);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetupParameterThrowsIfInputParametersAreInvalid()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ArgumentsValidator(new ComplexTestClass(), method, ArgumentsValidatorHelper.DefaultCreators, null);

            var validator = ArgumentsValidatorHelper.GetMethodArgumentsValidator(sut, nameof(ArgumentsValidator.SetupParameter), new object[] { "param1", new IExpectedException[] { ExpectedExceptionRules.NotNull } });

            validator.SetupParameter("paramName", ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmptyOrWhiteSpace)
                     .SetupParameter("rules", ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmpty)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetupParameterThrowsIfParameterNameIsNotValid()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ArgumentsValidator(new ComplexTestClass(), method, ArgumentsValidatorHelper.DefaultCreators, null);

            var ex = Assert.Throws<ArgumentException>(() => sut.SetupParameter("invalid", new ExpectedNoException()));

            Assert.Equal("paramName",
                         ex.ParamName);
            Assert.Equal("Parameter 'invalid' does not exists on the method. (Parameter 'paramName')",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetupParameterThrowsIfParameterNameHasAlreadyBeenAdded()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ArgumentsValidator(new ComplexTestClass(), method, ArgumentsValidatorHelper.DefaultCreators, null);

            sut.SetupParameter("param1",
                               new ExpectedNoException());

            var ex = Assert.Throws<ArgumentException>(() => sut.SetupParameter("param1", new ExpectedNoException()));

            Assert.Equal("paramName",
                         ex.ParamName);
            Assert.Equal("Parameter param1 has already been configured (Parameter 'paramName')",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateThrowsIfSomeParametersWereNotConfigured()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ArgumentsValidator(new ComplexTestClass(), method, ArgumentsValidatorHelper.DefaultCreators, null);

            sut.SetupParameter("param1",
                               new ExpectedNoException());

            var ex = Assert.Throws<InvalidOperationException>(() => sut.Validate());

            Assert.Equal("The following parameters have not been configured: param2, param3, param4, param5",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateUsesSpecifiedParameters()
        {
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithStringValidation));
            var parameters = new object[]
                             {
                                 "Freddie"
                             };
            var sut = new ArgumentsValidator(new SimpleTestClass(), method, ArgumentsValidatorHelper.DefaultCreators, parameters);

            sut.SetupParameter("param1",
                               ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmptyOrWhiteSpace)
               .Validate();
            
            // no exceptions
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateUsesGenerateDefaultParameters()
        {
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithStringValidation));
            var sut = new ArgumentsValidator(new SimpleTestClass(), method, ArgumentsValidatorHelper.DefaultCreators, null);

            sut.SetupParameter("param1",
                               ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmptyOrWhiteSpace)
               .Validate();
            
            // no exceptions
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateThrowsIfRuleFails()
        {
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithNoValidation));
            var sut = new ArgumentsValidator(new SimpleTestClass(), method, ArgumentsValidatorHelper.DefaultCreators, null);
            sut.SetupParameter("paramW",
                               ExpectedExceptionRules.NotNull,
                               ExpectedExceptionRules.NotEmptyOrWhiteSpace);

            var ex = Assert.Throws<Exception>(() => sut.Validate());

            Assert.Equal("Rule 'ExpectedNotNullException' for parameter 'paramW' was not respected. An exception was expected but none happened.",
                         ex.Message);

            // no exceptions
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateThrowsIfAParameterCannotBeGeneratedAutomatically()
        {
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithConcreteClass));
            var sut = new ArgumentsValidator(new SimpleTestClass(), method, ArgumentsValidatorHelper.DefaultCreators, null);
            sut.SetupParameter("paramO",
                               ExpectedExceptionRules.NotNull);

            var ex = Assert.Throws<Exception>(() => sut.Validate());

            Assert.Equal("Could not find an IObjectCreator for " + typeof(ComplexTestClass).FullName,
                         ex.Message);

            // no exceptions
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ValidateAsyncCallsAsyncCode()
        {
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithStringValidationAsync));
            var parameters = new object[]
                             {
                                 "Freddie"
                             };
            var sut = new ArgumentsValidator(new SimpleTestClass(), method, ArgumentsValidatorHelper.DefaultCreators, parameters);

            await sut.SetupParameter("param1",
                               ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmptyOrWhiteSpace)
               .ValidateAsync()
               .ConfigureAwait(false);;

            // no exceptions
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ValidateAsyncWithValueTaskThrowsException()
        {
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithStringValidationAsync));
            var parameters = new object[]
                             {
                                 (string)null
                             };
            var sut = new ArgumentsValidator(new SimpleTestClass(), method, ArgumentsValidatorHelper.DefaultCreators, parameters);

            var ex = await Assert.ThrowsAsync<Exception>(() => sut.SetupParameter("param1", ExpectedExceptionRules.None)
                                                                  .ValidateAsync()).ConfigureAwait(false);;

            Assert.Equal("Rule 'ExpectedNoException' for parameter 'param1' was not respected. An exception was thrown when no exception was expected",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ValidateAsyncCallsAsyncCodeWithGenericValueTask()
        {
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithStringValidationAsyncWithGenericValueTask));
            var parameters = new object[]
                             {
                                 "Freddie"
                             };
            var sut = new ArgumentsValidator(new SimpleTestClass(), method, ArgumentsValidatorHelper.DefaultCreators, parameters);

            await sut.SetupParameter("param1",
                                     ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmptyOrWhiteSpace)
               .ValidateAsync()
               .ConfigureAwait(false);;

            // no exceptions
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ValidateAsyncCallsAsyncCodeButNoValidation()
        {
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithNoValidationAsync));
            var parameters = new object[]
                             {
                                 "Freddie"
                             };
            var sut = new ArgumentsValidator(new SimpleTestClass(), method, ArgumentsValidatorHelper.DefaultCreators, parameters);

            await sut.SetupParameter("paramW",
                               ExpectedExceptionRules.None)
               .ValidateAsync()
               .ConfigureAwait(false);;

            // no exceptions
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ValidateAsyncCallsAsyncCodeWithNonGenericValueTask()
        {
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithNoValidationAsyncWithNonGenericValueTask));
            var parameters = new object[]
                             {  
                                 "Freddie"
                             };
            var sut = new ArgumentsValidator(new SimpleTestClass(), method, ArgumentsValidatorHelper.DefaultCreators, parameters);

            await sut.SetupParameter("paramW",
                               ExpectedExceptionRules.None)
               .ValidateAsync()
               .ConfigureAwait(false);;

            // no exceptions
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ValidateAsyncThrowsBecauseOfConstructor()
        {
            object[] parameters = new object[]
                                  {
                                      new ComplexTestClass(),
                                      typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod)),
                                      ArgumentsValidatorHelper.DefaultCreators,
                                      null
                                  };
            var validator = ArgumentsValidatorHelper.GetConstructorArgumentsValidator<ArgumentsValidator>(parameters);

            var ex = await Assert.ThrowsAsync<NotSupportedException>(() => validator.SetupParameter("targetObject", ExpectedExceptionRules.None)
                                                                                    .SetupParameter("method", ExpectedExceptionRules.NotNull)
                                                                                    .SetupParameter("objectCreators", ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmpty)
                                                                                    .SetupParameter("methodArguments", ExpectedExceptionRules.None)
                                                                                    .ValidateAsync()).ConfigureAwait(false);
            Assert.Equal("The requested method is not compatible with async/await. Please call the non async Validate method.",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ValidateAsyncThrowsBecauseOfNonAsyncMethod()
        {
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithStringValidation));
            var parameters = new object[]
                             {
                                 "Freddie"
                             };
            var sut = new ArgumentsValidator(new SimpleTestClass(), method, ArgumentsValidatorHelper.DefaultCreators, parameters);

            var ex = await Assert.ThrowsAsync<NotSupportedException>(() => sut.SetupParameter("param1",
                                                                                              ExpectedExceptionRules.NotNull,
                                                                                              ExpectedExceptionRules.NotEmptyOrWhiteSpace)
                                                                              .ValidateAsync())
                                 .ConfigureAwait(false);
            
            Assert.Equal("The requested method does not return a Task. Please call the non async Validate method.",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ValidateAsyncThrowsIfRuleFails()
        {
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithNoValidationAsync));
            var sut = new ArgumentsValidator(new SimpleTestClass(), method, ArgumentsValidatorHelper.DefaultCreators, null);
            sut.SetupParameter("paramW",
                               ExpectedExceptionRules.NotNull,
                               ExpectedExceptionRules.NotEmptyOrWhiteSpace);

            var ex = await Assert.ThrowsAsync<Exception>(() => sut.ValidateAsync()).ConfigureAwait(false);

            Assert.Equal("Rule 'ExpectedNotNullException' for parameter 'paramW' was not respected. An exception was expected but none happened.",
                         ex.Message);

            // no exceptions
        }
    }
}
