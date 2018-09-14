using System;
using NPlant.Console;
using NUnit.Framework;

namespace NPlant.Tests.Console
{
    [TestFixture]
    public class CliModelFixture
    {
        [TestCase(new[] { "Diagrams", "List" }, typeof(DiagramsListCommand), new string[0])]
        [TestCase(new[] { "diagrams", "list" }, typeof(DiagramsListCommand), new string[0])]
        [TestCase(new[] { "Diagrams", "List", "Arg1", "Arg2" }, typeof(DiagramsListCommand), new []{"Arg1", "Arg2"})]
        [TestCase(new[] { "diagrams", "list", "Arg1", "Arg2" }, typeof(DiagramsListCommand), new []{"Arg1", "Arg2"})]
        public void FindCommandType_Suite(string[] arguments, Type expectedType, string[] expectedCommandArgs)
        {
            var type = CliModel.FindCommandType(arguments, out var commandArgs);
            Assert.That(type, Is.EqualTo(expectedType));
            Assert.That(commandArgs.Length, Is.EqualTo(expectedCommandArgs.Length));

            for (var i = 0; i < commandArgs.Length; i++)
            {
                var commandArg = commandArgs[i];
                Assert.That(commandArg, Is.EqualTo(expectedCommandArgs[i]));
            }
        }
    }
}
