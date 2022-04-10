using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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
            object[] parameters =
            {
                new ComplexTestClass(),
                typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod)),
                null,
                ParametersValidatorHelper.DefaultCreators
            };
            var validator = ParametersValidatorHelper.GetConstructorParametersValidator<ParametersValidator>(parameters);

            validator.SetupParameter("targetObject",
                                     ExpectedExceptionRules.None)
                     .SetupParameter("method",
                                     ExpectedExceptionRules.NotNull)
                     .SetupParameter("methodParameters",
                                     ExpectedExceptionRules.None)
                     .SetupParameter("objectCreators",
                                     ExpectedExceptionRules.NotNull,
                                     ExpectedExceptionRules.NotEmpty)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ConstructorThrowIfMethodHasNoParameters()
        {
            var ex = Assert.Throws<ArgumentException>(() => new ParametersValidator(null,
                                                                                    MethodsHolder.GetMethodInfoWithNoParameters(),
                                                                                    null,
                                                                                    ParametersValidatorHelper.DefaultCreators));
            Assert.Equal("method",
                         ex.ParamName);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetParameterRulesReturnsTheConfiguredParameter()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(sut,
                                                                                   nameof(ParametersValidator.GetParameterRules),
                                                                                   new object[] { new ParametersValidationRules() });
            validator.SetupParameter("paramName",
                                     ExpectedExceptionRules.NotNull,
                                     ExpectedExceptionRules.NotEmptyOrWhiteSpace);

            var paramRules = validator.GetParameterRules("paramName");
            paramRules.Should().BeEquivalentTo(new[] { ExpectedExceptionRules.NotNull, ExpectedExceptionRules.NotEmptyOrWhiteSpace });
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetParameterRulesThrowsIfInputParametersAreInvalid()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(sut,
                                                                                   nameof(ParametersValidator.GetParameterRules),
                                                                                   new object[] { new ParametersValidationRules() });

            validator.SetupParameter("paramName",
                                     ExpectedExceptionRules.NotNull,
                                     ExpectedExceptionRules.NotEmptyOrWhiteSpace)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void GetParameterRulesThrowsIfRequestedParametersIsNotConfigured()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(sut,
                                                                                   nameof(ParametersValidator.GetParameterRules),
                                                                                   new object[] { new ParametersValidationRules() });

            var ex = Assert.Throws<KeyNotFoundException>(() => validator.GetParameterRules("invalid"));
            Assert.Equal("The given key 'invalid' was not present in the dictionary.",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetupEveryParametersWithCommonRules()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.MethodForDefaultRules));
            var validator = new ParametersValidator(new ComplexTestClass(),
                                                    method,
                                                    null,
                                                    ParametersValidatorHelper.DefaultCreators.Union(new[] { new ComplexTestClassObjectCreator() }).ToArray());

            validator.SetupAll(ParametersValidator.DefaultRules)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetupFallbackToNoneIfRulesWereNotDefineForDataType()
        {
            var validator = ParametersValidatorHelper.GetMethodParametersValidator(new ComplexTestClass(),
                                                                                   nameof(ComplexTestClass.MethodForDefaultRules),
                                                                                   new object[] { new ParametersValidationRules() });

            validator.SetupAll(new ParametersValidationRules());

            validator.GetParameterRules("param1").Should().BeEquivalentTo(new[] { ExpectedExceptionRules.None });
            validator.GetParameterRules("param2").Should().BeEquivalentTo(new[] { ExpectedExceptionRules.None });
            validator.GetParameterRules("param3").Should().BeEquivalentTo(new[] { ExpectedExceptionRules.None });
            validator.GetParameterRules("param4").Should().BeEquivalentTo(new[] { ExpectedExceptionRules.None });
            validator.GetParameterRules("param5").Should().BeEquivalentTo(new[] { ExpectedExceptionRules.None });
            validator.GetParameterRules("param6").Should().BeEquivalentTo(new[] { ExpectedExceptionRules.None });
            validator.GetParameterRules("param7").Should().BeEquivalentTo(new[] { ExpectedExceptionRules.None });
            validator.GetParameterRules("param8").Should().BeEquivalentTo(new[] { ExpectedExceptionRules.None });
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetupParameterReturnSameValidator()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);
            var returnedValidator = sut.SetupParameter("param1",
                                                       ExpectedExceptionRules.NotNull);
            Assert.Same(sut,
                        returnedValidator);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetupParameterThrowsIfInputParametersAreInvalid()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(sut,
                                                                                   nameof(ParametersValidator.SetupParameter),
                                                                                   new object[] { "param1", new[] { ExpectedExceptionRules.NotNull } });

            validator.SetupParameter("paramName",
                                     ExpectedExceptionRules.NotNull,
                                     ExpectedExceptionRules.NotEmptyOrWhiteSpace)
                     .SetupParameter("rules",
                                     ExpectedExceptionRules.NotNull,
                                     ExpectedExceptionRules.NotEmpty)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetupParameterThrowsIfParameterNameHasAlreadyBeenAdded()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);

            sut.SetupParameter("param1",
                               new ExpectedNoExceptionRule());

            var ex = Assert.Throws<ArgumentException>(() => sut.SetupParameter("param1",
                                                                               new ExpectedNoExceptionRule()));

            Assert.Equal("paramName",
                         ex.ParamName);
            Assert.Equal("Parameter param1 has already been configured (Parameter 'paramName')",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetupParameterThrowsIfParameterNameIsNotValid()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);

            var ex = Assert.Throws<ArgumentException>(() => sut.SetupParameter("invalid",
                                                                               new ExpectedNoExceptionRule()));

            Assert.Equal("paramName",
                         ex.ParamName);
            Assert.Equal("Parameter 'invalid' does not exists on the method. (Parameter 'paramName')",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetupReturnSameValidator()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);
            var returnedValidator = sut.SetupAll(ParametersValidator.DefaultRules);
            Assert.Same(sut,
                        returnedValidator);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetupShouldNotThrowIfParameterWasAlreadyConfigured()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.MethodForDefaultRules));
            var validator = new ParametersValidator(new ComplexTestClass(),
                                                    method,
                                                    null,
                                                    ParametersValidatorHelper.DefaultCreators.Union(new[] { new ComplexTestClassObjectCreator() }).ToArray());

            validator.SetupParameter("param1",
                                     ExpectedExceptionRules.NotNull)
                     .SetupAll(ParametersValidator.DefaultRules)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void SetupThrowsIfInputParametersAreInvalid()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(sut,
                                                                                   nameof(ParametersValidator.SetupAll),
                                                                                   new object[] { new ParametersValidationRules() });

            validator.SetupParameter("validationRules",
                                     ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void UpdateParameterChangesTheConfiguredRules()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);
            sut.SetupParameter("param1",
                               ExpectedExceptionRules.NotNull);
            sut.UpdateParameter("param1",
                                ExpectedExceptionRules.NotEmptyOrWhiteSpace);
            sut.GetParameterRules("param1").Should().BeEquivalentTo(new[] { ExpectedExceptionRules.NotEmptyOrWhiteSpace });
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void UpdateParameterReturnSameValidator()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);
            sut.SetupParameter("param1",
                               ExpectedExceptionRules.NotNull);
            var returnedValidator = sut.UpdateParameter("param1",
                                                        ExpectedExceptionRules.NotEmptyOrWhiteSpace);
            Assert.Same(sut,
                        returnedValidator);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void UpdateParameterThrowsIfInputParametersAreInvalid()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);

            var validator = ParametersValidatorHelper.GetMethodParametersValidator(sut,
                                                                                   nameof(ParametersValidator.UpdateParameter),
                                                                                   new object[] { "param1", new[] { ExpectedExceptionRules.NotNull } });

            validator.SetupParameter("paramName",
                                     ExpectedExceptionRules.NotNull,
                                     ExpectedExceptionRules.NotEmptyOrWhiteSpace)
                     .SetupParameter("rules",
                                     ExpectedExceptionRules.NotNull,
                                     ExpectedExceptionRules.NotEmpty)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void UpdateParameterThrowsIfParameterNameHasNotBeenAdded()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);

            var ex = Assert.Throws<KeyNotFoundException>(() => sut.UpdateParameter("param1",
                                                                                   new ExpectedNoExceptionRule()));

            Assert.Equal("The given key 'param1' was not present in the dictionary.",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void UpdateParameterThrowsIfParameterNameIsNotValid()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);

            var ex = Assert.Throws<ArgumentException>(() => sut.UpdateParameter("invalid",
                                                                                new ExpectedNoExceptionRule()));

            Assert.Equal("paramName",
                         ex.ParamName);
            Assert.Equal("Parameter 'invalid' does not exists on the method. (Parameter 'paramName')",
                         ex.Message);
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
            var sut = new ParametersValidator(new SimpleTestClass(),
                                              method,
                                              parameters,
                                              ParametersValidatorHelper.DefaultCreators);

            await sut.SetupParameter("param1",
                                     ExpectedExceptionRules.NotNull,
                                     ExpectedExceptionRules.NotEmptyOrWhiteSpace)
                     .ValidateAsync()
                     .ConfigureAwait(false);
            ;

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
            var sut = new ParametersValidator(new SimpleTestClass(),
                                              method,
                                              parameters,
                                              ParametersValidatorHelper.DefaultCreators);

            await sut.SetupParameter("paramW",
                                     ExpectedExceptionRules.None)
                     .ValidateAsync()
                     .ConfigureAwait(false);
            ;

            // no exceptions
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
            var sut = new ParametersValidator(new SimpleTestClass(),
                                              method,
                                              parameters,
                                              ParametersValidatorHelper.DefaultCreators);

            await sut.SetupParameter("param1",
                                     ExpectedExceptionRules.NotNull,
                                     ExpectedExceptionRules.NotEmptyOrWhiteSpace)
                     .ValidateAsync()
                     .ConfigureAwait(false);
            ;

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
            var sut = new ParametersValidator(new SimpleTestClass(),
                                              method,
                                              parameters,
                                              ParametersValidatorHelper.DefaultCreators);

            await sut.SetupParameter("paramW",
                                     ExpectedExceptionRules.None)
                     .ValidateAsync()
                     .ConfigureAwait(false);
            ;

            // no exceptions
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ValidateAsyncThrowsBecauseOfConstructor()
        {
            object[] parameters =
            {
                new ComplexTestClass(),
                typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod)),
                ParametersValidatorHelper.DefaultCreators,
                null
            };
            var validator = ParametersValidatorHelper.GetConstructorParametersValidator<ParametersValidator>(parameters);

            var ex = await Assert.ThrowsAsync<NotSupportedException>(() => validator.SetupParameter("targetObject",
                                                                                                    ExpectedExceptionRules.None)
                                                                                    .SetupParameter("method",
                                                                                                    ExpectedExceptionRules.NotNull)
                                                                                    .SetupParameter("objectCreators",
                                                                                                    ExpectedExceptionRules.NotNull,
                                                                                                    ExpectedExceptionRules.NotEmpty)
                                                                                    .SetupParameter("methodParameters",
                                                                                                    ExpectedExceptionRules.None)
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
            var sut = new ParametersValidator(new SimpleTestClass(),
                                              method,
                                              parameters,
                                              ParametersValidatorHelper.DefaultCreators);

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
        public void ValidateThrowsBecauseOfAsyncMethod()
        {
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithStringValidationAsync));
            var parameters = new object[]
                             {
                                 "Freddie"
                             };
            var sut = new ParametersValidator(new SimpleTestClass(),
                                              method,
                                              parameters,
                                              ParametersValidatorHelper.DefaultCreators);

            var ex = Assert.Throws<NotSupportedException>(() => sut.SetupParameter("param1",
                                                                                   ExpectedExceptionRules.NotNull,
                                                                                   ExpectedExceptionRules.NotEmptyOrWhiteSpace)
                                                                   .Validate());

            Assert.Equal("The requested method does returns a Task. Please call the ValidateAsync method.",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ValidateAsyncThrowsIfNoParametersWereConfigured()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.ValidateAsync()).ConfigureAwait(false);

            Assert.Equal("No parameters were configured for validation. Call SetupAll or SetupParameter before calling Validate/ValidateAsync.",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public async Task ValidateAsyncThrowsIfRuleFails()
        {
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithNoValidationAsync));
            var sut = new ParametersValidator(new SimpleTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);
            sut.SetupParameter("paramW",
                               ExpectedExceptionRules.NotNull,
                               ExpectedExceptionRules.NotEmptyOrWhiteSpace);

            var ex = await Assert.ThrowsAsync<ParameterRuleException>(() => sut.ValidateAsync()).ConfigureAwait(false);

            Assert.Equal("Rule 'ExpectedNotNullExceptionRule' for parameter 'paramW' was not respected. An exception was expected but none happened.",
                         ex.Message);

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
                                 null
                             };
            var sut = new ParametersValidator(new SimpleTestClass(),
                                              method,
                                              parameters,
                                              ParametersValidatorHelper.DefaultCreators);

            var ex = await Assert.ThrowsAsync<ParameterRuleException>(() => sut.SetupParameter("param1",
                                                                                               ExpectedExceptionRules.None)
                                                                               .ValidateAsync()).ConfigureAwait(false);

            Assert.StartsWith("Rule 'ExpectedNoExceptionRule' for parameter 'param1' was not respected. An exception was thrown when no exception was expected",
                              ex.Message,
                              StringComparison.Ordinal);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateThrowsIfAParameterCannotBeGeneratedAutomatically()
        {
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithConcreteClass));
            var sut = new ParametersValidator(new SimpleTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);
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
        public void ValidateThrowsIfNoParametersWereConfigured()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);

            var ex = Assert.Throws<InvalidOperationException>(() => sut.Validate());

            Assert.Equal("No parameters were configured for validation. Call SetupAll or SetupParameter before calling Validate/ValidateAsync.",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateThrowsIfRuleFails()
        {
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithNoValidation));
            var sut = new ParametersValidator(new SimpleTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);
            sut.SetupParameter("paramW",
                               ExpectedExceptionRules.NotNull,
                               ExpectedExceptionRules.NotEmptyOrWhiteSpace);

            var ex = Assert.Throws<ParameterRuleException>(() => sut.Validate());

            Assert.Equal("Rule 'ExpectedNotNullExceptionRule' for parameter 'paramW' was not respected. An exception was expected but none happened.",
                         ex.Message);

            // no exceptions
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateThrowsIfSomeParametersWereNotConfigured()
        {
            var method = typeof(ComplexTestClass).GetMethod(nameof(ComplexTestClass.SomeMethod));
            var sut = new ParametersValidator(new ComplexTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);

            sut.SetupParameter("param1",
                               new ExpectedNoExceptionRule());

            var ex = Assert.Throws<UnconfiguredArgumentsException>(() => sut.Validate());

            Assert.Equal("The following arguments have not been configured: param2, param3, param4, param5.",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void ValidateUsesGenerateDefaultParameters()
        {
            var method = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.MethodWithStringValidation));
            var sut = new ParametersValidator(new SimpleTestClass(),
                                              method,
                                              null,
                                              ParametersValidatorHelper.DefaultCreators);

            sut.SetupParameter("param1",
                               ExpectedExceptionRules.NotNull,
                               ExpectedExceptionRules.NotEmptyOrWhiteSpace)
               .Validate();

            // no exceptions
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
            var sut = new ParametersValidator(new SimpleTestClass(),
                                              method,
                                              parameters,
                                              ParametersValidatorHelper.DefaultCreators);

            sut.SetupParameter("param1",
                               ExpectedExceptionRules.NotNull,
                               ExpectedExceptionRules.NotEmptyOrWhiteSpace)
               .Validate();

            // no exceptions
        }
    }
}