using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using NPlant.Generation;
using NPlant.Generation.ClassDiagraming;
using Con=System.Console;

namespace NPlant.Console
{
    public class DiagramsGenerateCommand : CommandLineCommand
    {
        public DiagramsGenerateCommand()
        {
            this.Format = "png";
            this.Output = Directory.GetCurrentDirectory();
        }

        public override IEnumerable<string> Usage()
        {
            yield return "NPlant.Console.exe diagrams generate ASSEMBLY";
            yield return "";
            yield return "Arguments:";
            yield return "  ASSEMBLY          The path to the assembly to load diagrams from";
            yield return "";
            yield return "Options:";
            yield return "  --java:<path>     The path the java executable";
            yield return "  --jar:<path>      The path to the plantuml.jar.";
            yield return "                    Default will be to download it if it is not present in the execution directory";
            yield return "  --option:<path>   Path to the directory where you would like the files to be written";
            yield return "                    The default is the current working directory.";
            yield return "  --filter:<path>   If you would like to specify a filter for which diagrams are rendered.  This value";
            yield return "                    Will be matched against the *front* of diagram's name.";
        }

        [Argument(1)]
        public string Assembly { get; protected set; }

        public string Java { get; protected set; }

        public string Jar { get; protected set; }

        public string Output { get; protected set; }

        public string Filter { get; protected set; }


        private string _format;
        private ImageFormat _imageFormat;

        public string Format
        {
            get => _format;
            protected set
            {
                _format = value;
                _imageFormat = null;

                if (!string.Equals("nplant", _format))
                {
                    var converter = TypeDescriptor.GetConverter(typeof(ImageFormat));
                    _imageFormat = (ImageFormat)converter.ConvertFromInvariantString(_format);
                }
            }
        }

        public ImageFormat GetImageFormat()
        {
            return _imageFormat;
        }

        public override int Run()
        {
            this.Jar = SetupPlantUml(this.Jar);

            foreach (var matchingDiagram in this.DiscoveredDiagrams(this.Assembly, this.Filter))
            {
                Con.WriteLine($" * generating diagram {matchingDiagram.Diagram.Name}...");
                string diagramText = BufferedClassDiagramGenerator.GetDiagramText(matchingDiagram.Diagram);
                ImageFileGenerationModel gen = new ImageFileGenerationModel(diagramText, matchingDiagram.Diagram.Name, this.Java, this.Jar);

                DirectoryInfo outputDirectory = new DirectoryInfo(this.Output);

                if (!outputDirectory.Exists)
                    outputDirectory.Create();

                string path = Path.Combine(outputDirectory.FullName, $"{gen.DiagramName}.{this.Format}");
                ImageFormat format = this.GetImageFormat();

                if (format == null)
                {
                    File.WriteAllText(path, diagramText);
                }
                else
                {
                    NPlantImage nplantImage = new NPlantImage(gen.JavaPath, gen.Invocation);
                    nplantImage.Save(gen.DiagramText, gen.DiagramName, path, format);
                }

                Con.WriteLine($" * diagram '{gen.DiagramName}' saved to '{path}'");
            }

            return 0;
        }
    }
}