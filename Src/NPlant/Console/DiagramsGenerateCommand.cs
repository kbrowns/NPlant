using System.Collections.Generic;
using System.Linq;
using NPlant.Generation.ClassDiagraming;

namespace NPlant.Console
{
    public class DiagramsGenerateCommand : CliCommand
    {
        [CliArgument(1)]
        public string Assembly { get; protected set; }

        public string Java { get; protected set; }

        public string Jar { get; protected set; }

        public string Output { get; protected set; }

        public string Filter { get; protected set; }

        [CliArgument(2, Allowed = new[] { "nplant", "png", "jpg", "gif" })]
        public string Format { get; protected set; }

        public override int Run()
        {
            this.Jar = SetupPlantUml(this.Jar);

            var models = this.DiscoveredDiagrams(this.Assembly, this.Filter).Select(x =>
            {
                string diagramText = BufferedClassDiagramGenerator.GetDiagramText(x.Diagram);
                return new ImageFileGenerationModel(diagramText, x.Diagram.Name, this.GetJavaPath(this.Java), this.Jar);
            });

            WriteDiagramsToDisk(models, this.Format, this.Output);

            return 0;
        }

        public override IEnumerable<string> Usage()
        {
            yield return "NPlant.Console.exe diagrams generate ASSEMBLY FORMAT";
            yield return "";
            yield return "Arguments:";
            yield return "  ASSEMBLY          The path to the assembly to load diagrams from";
            yield return "  FORMAT            The desired output format:  png, jpeg, or gif.";
            yield return "                    Unlike other command, this does not support .nplant as a format as that file";
            yield return "                    is the input to this command.";
            yield return "";
            yield return "Options:";
            yield return "  --java:<path>     The path the java executable";
            yield return "  --jar:<path>      The path to the plantuml.jar.";
            yield return "                    Default will be to download it if it is not present in the execution directory";
            yield return "  --output:<path>   PathW to the directory where you would like the files to be written";
            yield return "                    The default is the current working directory.";
            yield return "  --filter:<path>   If you would like to specify a filter for which diagrams are rendered.  This value";
            yield return "                    Will be matched against the *front* of diagram's name.";
        }
    }
}