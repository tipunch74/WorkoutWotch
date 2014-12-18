﻿namespace WorkoutWotch.UnitTests.Models.Parsers
{
    using System;
    using Kent.Boogaart.PCLMock;
    using NUnit.Framework;
    using Sprache;
    using WorkoutWotch.Models.Parsers;
    using WorkoutWotch.UnitTests.Services.Container.Mocks;

    [TestFixture]
    public class ExerciseProgramParserFixture
    {
        [Test]
        public void get_parser_throws_if_container_service_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => ExerciseProgramParser.GetParser(null));
        }

        [TestCase("# foo\n", "foo")]
        [TestCase("# Foo\n", "Foo")]
        [TestCase("# Foo bar\n", "Foo bar")]
        [TestCase("# !@$%^&*()-_=+[{]};:'\",<.>/?\n", "!@$%^&*()-_=+[{]};:'\",<.>/?")]
        [TestCase("#    \t Foo   bar  \t \n", "Foo   bar")]
        public void can_parse_name(string input, string expectedName)
        {
            var result = ExerciseProgramParser
                .GetParser(new ContainerServiceMock(MockBehavior.Loose))
                .Parse(input);

            Assert.NotNull(result);
            Assert.AreEqual(expectedName, result.Name);
        }

        [TestCase("# ignore\n", 0)]
        [TestCase("# ignore\n## Exercise 1\n* 1 set x 1 rep\n", 1)]
        [TestCase("# ignore\n## Exercise 1\n* 1 set x 1 rep\n## Exercise 2\n* 1 set x 1 rep\n", 2)]
        [TestCase("# ignore\n## Exercise 1\n* 1 set x 1 rep\n  \n  \t  \n\n## Exercise 2\n* 1 set x 1 rep\n", 2)]
        public void can_parse_exercises(string input, int expectedExerciseCount)
        {
            var result = ExerciseProgramParser
                .GetParser(new ContainerServiceMock(MockBehavior.Loose))
                .Parse(input);

            Assert.NotNull(result);
            Assert.AreEqual(expectedExerciseCount, result.Exercises.Count);
        }

        [TestCase("## two hashes\n")]
        [TestCase("#\n")]
        [TestCase("#no space after hash\n")]
        [TestCase("  # leading whitespace\n")]
        public void cannot_parse_invalid_input(string input)
        {
            var result = ExerciseProgramParser
                .GetParser(new ContainerServiceMock(MockBehavior.Loose))(new Input(input));
            Assert.True(!result.WasSuccessful || !result.Remainder.AtEnd);
        }
    }
}