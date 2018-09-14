using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using NPlant.Core;
using NPlant.Generation;

namespace NPlant.Console
{
    public abstract class CliCommand
    {
        private static IEnumerable<Type> _availableTypes = null;

        public abstract int Run();

        public abstract IEnumerable<string> Usage();

        public bool Help { get; protected set; }

        public static IEnumerable<Type> AvailableCommandTypes
        {
            get
            {
                return _availableTypes ?? (_availableTypes = typeof(CliCommand).Assembly.GetExportedTypes()
                           .Where(x => typeof(CliCommand).IsAssignableFrom(x) &&
                                       x.Namespace == typeof(CliCommand).Namespace &&
                                       !x.IsAbstract));
            }
        }

        public bool Debugger { get; protected set; }

        public bool Debug { get; protected set; }

        protected string GetJavaPath(string javaPath)
        {
            if (javaPath.IsNullOrEmpty())
                javaPath = ConsoleEnvironment.GetSettings().JavaPath;

            return javaPath;
        }

        protected string SetupPlantUml(string jarPath)
        {
            if (string.IsNullOrEmpty(jarPath))
                jarPath = Path.Combine(ConsoleEnvironment.ExecutionDirectory, "plantuml.jar");

            if (!string.IsNullOrEmpty(jarPath) && File.Exists(jarPath))
            {
                LogDebug(() => $"plantuml.jar download skipped - found at {jarPath}");
            }
            else
            {
                try
                {
                    LogDebug(() => $"Atempting plantuml.jar download...");
                    WebClient client = new WebClient();
                    client.DownloadFile("http://sourceforge.net/projects/plantuml/files/plantuml.jar/download", jarPath);
                }
                catch (WebException ex)
                {
                    throw new ApplicationException("Failed to download plantuml.jar.  If the internet is not available, please place a copy of this file in the execution directory.", ex);
                }

                LogDebug(() => $"Downloaded plantuml.jar to {jarPath}");
            }

            return jarPath;
        }

        protected IEnumerable<DiscoveredDiagram> DiscoveredDiagrams(string assemblyPath, string filter = null)
        {
            var assemblyLoader = new NPlantAssemblyLoader();
            Assembly assembly = assemblyLoader.Load(assemblyPath);

            var diagramLoader = new NPlantDiagramLoader();

            var diagrams = diagramLoader.Load(assembly);

            if (filter != null && !filter.IsNullOrEmpty())
                diagrams = diagrams.Where(diagram => diagram.Diagram.Name.StartsWith(filter));

            return diagrams;
        }

        protected void LogDebug(Func<string> msg)
        {
            if (this.Debug)
            {
                System.Console.WriteLine($"[debug] ===> {msg()}");
            }
        }

        protected static void WriteDiagramsToDisk(IEnumerable<ImageFileGenerationModel> models, string format, string output)
        {
            if (string.IsNullOrEmpty(output))
                output = Directory.GetCurrentDirectory();

            DirectoryInfo outputDirectory = new DirectoryInfo(output);

            if (!outputDirectory.Exists)
                outputDirectory.Create();

            foreach (var model in models)
            {
                System.Console.WriteLine($" * generating diagram {model.DiagramName}...");

                string path = Path.Combine(outputDirectory.FullName, $"{model.DiagramName}.{format}");

                if (format.ToLower() == "nplant")
                {
                    File.WriteAllText(path, model.DiagramText);
                }
                else
                {
                    NPlantImage nplantImage = new NPlantImage(model.JavaPath, model.Invocation);
                    nplantImage.Save(model.DiagramText, model.DiagramName, path, format);
                }

                System.Console.WriteLine($" * diagram '{model.DiagramName}' saved to '{path}'");
            }
        }
    }
}