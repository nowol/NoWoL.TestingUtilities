using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NoWoL.TestingUtilities.ExpectedExceptions;
using NoWoL.TestingUtilities.ObjectCreators;
using Xunit;

namespace NoWoL.TestingUtilities.Tests.ObjectCreators
{
    public class HelpersTests
    {
        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateArrayOfIntegers()
        {
            var result = CreatorHelpers.CreateArray(typeof(int),
                                                    ParametersValidatorHelper.DefaultCreators) as int[];
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(0,
                         result[0]);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateArrayThrowIfInputParametersAreInvalid()
        {
            var method = typeof(CreatorHelpers).GetMethod(nameof(CreatorHelpers.CreateArray),
                                                          BindingFlags.NonPublic | BindingFlags.Static);
            var validator = ParametersValidatorHelper.GetMethodParametersValidator(null,
                                                                                   method,
                                                                                   new object[] { typeof(int), ParametersValidatorHelper.DefaultCreators });

            validator.SetupParameter("type",
                                     ExpectedExceptionRules.NotNull)
                     .SetupParameter("objectCreators",
                                     ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateItemFromTypeCreatesAnInteger()
        {
            var result = (int)CreatorHelpers.CreateItemFromType(typeof(int),
                                                                ParametersValidatorHelper.DefaultCreators);
            Assert.Equal(0,
                         result);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateItemFromTypeThrowForUnknownType()
        {
            var ex = Assert.Throws<NotSupportedException>(() => CreatorHelpers.CreateItemFromType(typeof(ComplexTestClass),
                                                                                                  ParametersValidatorHelper.DefaultCreators));
            Assert.Equal("No object creators have been registered to handle type '" + typeof(ComplexTestClass).FullName + "'",
                         ex.Message);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateItemFromTypeThrowIfInputParametersAreInvalid()
        {
            var method = typeof(CreatorHelpers).GetMethod(nameof(CreatorHelpers.CreateItemFromType),
                                                          BindingFlags.NonPublic | BindingFlags.Static);
            var validator = ParametersValidatorHelper.GetMethodParametersValidator(null,
                                                                                   method,
                                                                                   new object[] { typeof(int), ParametersValidatorHelper.DefaultCreators });

            validator.SetupParameter("type",
                                     ExpectedExceptionRules.NotNull)
                     .SetupParameter("objectCreators",
                                     ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateListOfIntegers()
        {
            var result = CreatorHelpers.CreateList(typeof(int),
                                                   ParametersValidatorHelper.DefaultCreators) as IList;
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(0,
                         result[0]);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void CreateListThrowIfInputParametersAreInvalid()
        {
            var method = typeof(CreatorHelpers).GetMethod(nameof(CreatorHelpers.CreateList),
                                                          BindingFlags.NonPublic | BindingFlags.Static);
            var validator = ParametersValidatorHelper.GetMethodParametersValidator(null,
                                                                                   method,
                                                                                   new object[] { typeof(int), ParametersValidatorHelper.DefaultCreators });

            validator.SetupParameter("type",
                                     ExpectedExceptionRules.NotNull)
                     .SetupParameter("objectCreators",
                                     ExpectedExceptionRules.NotNull)
                     .Validate();
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void TryCreateObjectReturnsFalseIfObjectCannotBeCreated()
        {
            var result = CreatorHelpers.TryCreateObject(typeof(int),
                                                        new List<IObjectCreator>(),
                                                        out var obj);
            Assert.False(result);
            Assert.Null(obj);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void TryCreateObjectReturnsTrueIfObjectCanBeCreated()
        {
            var result = CreatorHelpers.TryCreateObject(typeof(int),
                                                        new List<IObjectCreator> { new ValueTypeCreator() },
                                                        out var obj);
            Assert.True(result);
            Assert.Equal(0,
                         obj);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void TryCreateObjectThrowsIfObjectCreatorsIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => CreatorHelpers.TryCreateObject(typeof(int),
                                                                                               null,
                                                                                               out _));
            Assert.Equal("objectCreators",
                         ex.ParamName);
        }

        [Fact]
        [Trait("Category",
               "Unit")]
        public void TryCreateObjectThrowsIfTypeIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => CreatorHelpers.TryCreateObject(null,
                                                                                               ParametersValidatorHelper.DefaultCreators,
                                                                                               out _));
            Assert.Equal("type",
                         ex.ParamName);
        }

        // TryCreateObject
    }
}