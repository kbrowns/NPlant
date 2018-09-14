using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPlant.Exceptions;

namespace NPlant.Console
{
    public class DiagramsRenderCommand : CliCommand
    {
        public string Java { get; protected set; }

        public string Jar { get; protected set; }

        public string Output { get; protected set; }

        [CliArgument(1)]
        public string Expression { get; set; }

        [CliArgument(2, Allowed = new[] { "png", "jpg", "gif" })]
        public string Format { get; protected set; }

        public override int Run()
        {
            this.Jar = this.SetupPlantUml(this.Jar);

            LogDebug(() => $"Expression:  {this.Expression}");

            var expression = Path.GetFullPath(Path.GetDirectoryName(this.Expression));

            LogDebug(() => $"Expanded: {expression}");

            if (string.IsNullOrEmpty(expression))
                throw new NPlantConsoleUsageException($"Invalid EXPRESSION '{this.Expression}' - could not find rooted directory.");

            DirectoryInfo directory = new DirectoryInfo(expression);
            string fileExpression = Path.GetFileName(this.Expression);

            LogDebug(() => $"FileExpression:  {fileExpression}");

            if (string.IsNullOrEmpty(fileExpression))
                fileExpression = "*.nplant";

            var files = directory.GetFiles(fileExpression);

            LogDebug(() => $"File Match count:  {files.Length}");

            var models = files.Select(x =>
            {
                string diagramText = File.ReadAllText(x.FullName);
                return new ImageFileGenerationModel(diagramText, Path.GetFileNameWithoutExtension(x.FullName), this.GetJavaPath(this.Java), this.Jar);
            });

            WriteDiagramsToDisk(models, this.Format, this.Output);

            return 0;
        }

        public override IEnumerable<string> Usage()
        {
            yield return "NPlant.Console.exe diagrams render EXPRESSION FORMAT";
            yield return "";
            yield return "Arguments:";
            yield return "  EXPRESSION        Search expression for finding text files containing plantuml syntax";
            yield return "  FORMAT            The desired output format:  png, jpeg, or gif.";
            yield return "                    Unlike other command, this does not support .nplant as a format as that file";
            yield return "                    is the input to this command.";
            yield return "";
            yield return "Options:";
            yield return "  --java:<path>     The path the java executable";
            yield return "  --jar:<path>      The path to the plantuml.jar.";
            yield return "                    Default will be to download it if it is not present in the execution directory";
            yield return "  --output:<path>   Path to the directory where you would like the files to be written";
            yield return "                    The default is the current working directory.";
        }
    }
}