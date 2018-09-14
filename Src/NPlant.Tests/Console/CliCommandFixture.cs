using System.Linq;
using NPlant.Console;
using NUnit.Framework;

namespace NPlant.Tests.Console
{
    [TestFixture]
    public class CliCommandFixture
    {
        [Test]
        public void AvailableCommandTypes_Should_Return_Types()
        {
            Assert.That(CliCommand.AvailableCommandTypes.ToArray().Length, Is.GreaterThan(0));
        }
    }
}
