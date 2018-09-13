using System.Linq;
using NPlant.Console;
using NUnit.Framework;

namespace NPlant.Tests.Console
{
    [TestFixture]
    public class CommandLineCommandFixture
    {
        [Test]
        public void AvailableCommandTypes_Should_Return_Types()
        {
            Assert.That(CommandLineCommand.AvailableCommandTypes.ToArray().Length, Is.GreaterThan(0));
        }
    }
}
