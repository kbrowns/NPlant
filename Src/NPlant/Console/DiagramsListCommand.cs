using System.Collections.Generic;
using Con=System.Console;

namespace NPlant.Console
{
    public class DiagramsListCommand : CommandLineCommand
    {
        [Argument(1)]
        public string Assembly { get; set; }

        public override int Run()
        {
            var diagrams = DiscoveredDiagrams(this.Assembly);

            foreach (var diagram in diagrams)
            {
                Con.WriteLine($" * {diagram.Diagram.Name}");
            }

            return 0;
        }

        public override IEnumerable<string> Usage()
        {
            yield return "NPlant.Console.exe diagrams list ASSEMBLY";
            yield return "";
            yield return "Arguments:";
            yield return "  ASSEMBLY          The path to the assembly to load diagrams from";
            yield return "";
        }
    }
}