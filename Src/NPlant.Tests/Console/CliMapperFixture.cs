using System.Collections.Generic;
using System.Drawing.Imaging;
using NPlant.Console;
using NPlant.Exceptions;
using NUnit.Framework;

namespace NPlant.Tests.Console
{
    [TestFixture]
    public class CliMapperFixture
    {
        [TestCase("foo")]
        [TestCase("foo:bar")]
        [TestCase("-foo")]
        [TestCase("-foo:bar")]
        public void Invalid_Arg_Syntax_Should_Throw(string invalidArg)
        {
            Assert.Throws<NPlantConsoleUsageException>(() =>
            {
                var subject = new StringsSubject();
                CliMapper.Map(subject, new string[0],  new[] { invalidArg });
            });
        }

        [Test]
        public void Can_Handle_Path_Values_As_Options()
        {
            var subject = new StringsSubject();
            CliMapper.Map(subject, new string[0],  new[] { "--foo:\"C:\\a\\b c\\d\"", "--bar:barrrr" });
            Assert.That(subject.Foo, Is.EqualTo("C:\\a\\b c\\d"));
            Assert.That(subject.Bar, Is.EqualTo("barrrr"));
        }

        [Test]
        public void Invalid_Arg_Value_Should_Throw()
        {
            Assert.Throws<NPlantConsoleUsageException>(() =>
            {
                var subject = new ComplexConverterSubject();
                CliMapper.Map(subject, new string[0],  new[] { "--foo:bar" });
            }, "Argument '--foo' has an invalid value 'bar'");
        }

        [Test]
        public void Missing_Required_Value_Should_Throw()
        {
            Assert.Throws<NPlantConsoleUsageException>(() =>
            {
                var subject = new ArgumentSubject();
                CliMapper.Map(subject, new string[0],  new string[] { });
            }, "Expected argument '--foo', but was not received");
        }



        [Test]
        public void Can_Map_Strings()
        {
            var subject = new StringsSubject();
            CliMapper.Map(subject, new string[0], new[] { "--foo:bar", "--bar:baz" });
            Assert.That(subject.Foo, Is.EqualTo("bar"));
            Assert.That(subject.Bar, Is.EqualTo("baz"));
            Assert.That(subject.Baz, Is.False);
        }

        [Test]
        public void Can_Map_Booleans_Switch_Or_Value()
        {
            var subject = new BooleansSubject();
            CliMapper.Map(subject, new string[0], new[] { "--foo"});
            Assert.That(subject.Foo, Is.True);
            Assert.That(subject.Bar, Is.Null);
            Assert.That(subject.Baz, Is.False);

            CliMapper.Map(subject, new string[0], new[] { "--foo:true" });
            Assert.That(subject.Foo, Is.True);
            Assert.That(subject.Bar, Is.Null);
            Assert.That(subject.Baz, Is.False);

            CliMapper.Map(subject, new string[0], new[] { "--foo:false" });
            Assert.That(subject.Foo, Is.False);
            Assert.That(subject.Bar, Is.Null);
            Assert.That(subject.Baz, Is.False);
        }

        [Test]
        public void Can_Map_Nullable_Booleans()
        {
            var subject = new BooleansSubject();
            CliMapper.Map(subject, new string[0], new[] { "--bar" });
            Assert.That(subject.Foo, Is.False);
            Assert.That(subject.Bar.GetValueOrDefault(), Is.True);
            Assert.That(subject.Baz, Is.False);

            CliMapper.Map(subject, new string[0], new[] { "--bar:true" });
            Assert.That(subject.Foo, Is.False);
            Assert.That(subject.Bar.GetValueOrDefault(), Is.True);
            Assert.That(subject.Baz, Is.False);

            CliMapper.Map(subject, new string[0], new[] { "--bar:false" });
            Assert.That(subject.Foo, Is.False);
            Assert.That(subject.Bar.GetValueOrDefault(), Is.False);
            Assert.That(subject.Baz, Is.False);
        }

        [Test]
        public void Can_Map_Complex_Converters()
        {
            var subject = new ComplexConverterSubject();
            CliMapper.Map(subject, new string[0], new []{"--foo:Png"});

            Assert.That(subject.Foo, Is.EqualTo(ImageFormat.Png));

            CliMapper.Map(subject, new string[0], new[] { "--foo:Jpeg" });

            Assert.That(subject.Foo, Is.EqualTo(ImageFormat.Jpeg));

            CliMapper.Map(subject, new string[0], new[] { "--foo:gif" });

            Assert.That(subject.Foo, Is.EqualTo(ImageFormat.Gif));
        }

        public class StubCliCommand : CliCommand
        {
            public override int Run()
            {
                return 0;
            }

            public override IEnumerable<string> Usage()
            {
                yield return "";
            }
        }

        public class StringsSubject : StubCliCommand
        {
            public string Foo { get; protected set; }
            public string Bar { get; protected set; }
            public bool Baz { get; protected set; }

        }

        public class BooleansSubject : StubCliCommand
        {
            public bool Foo { get; protected set; }
            public bool? Bar { get; protected set; }
            public bool Baz { get; protected set; }
        }

        public class ArgumentSubject : StubCliCommand
        {
            [CliArgument(1)]
            public string Foo { get; protected set; }
            public string Bar { get; protected set; }
            public bool Baz { get; protected set; }
        }


        public class ComplexConverterSubject : StubCliCommand
        {
            public ImageFormat Foo { get; protected set; }
        }
    }
}
