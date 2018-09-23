using System;
using NPlant.Console;
using NUnit.Framework;

namespace NPlant.Tests.Console
{
    [TestFixture]
    public class DiagramListCommandFixture
    {
        [Test]
        public void DiagramListCommand_Should_Run_Successfully()
        {
            DiagramsListCommand command = new DiagramsListCommand();

            string codeBase = typeof(DiagramsListCommand).Assembly.CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);

            command.Assembly = Uri.UnescapeDataString(uri.Path);
            Assert.That(command.Run(), Is.EqualTo(0));
        }
    }
}