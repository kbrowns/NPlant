using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NPlant.Core;
using NPlant.Generation;

namespace NPlant.Console
{
    public abstract class CommandLineCommand
    {
        private static IEnumerable<Type> _availableTypes = null;

        public abstract int Run();

        public abstract IEnumerable<string> Usage();

        public bool Help { get; protected set; }

        public static IEnumerable<Type> AvailableCommandTypes
        {
            get
            {
                return _availableTypes ?? (_availableTypes = typeof(CommandLineCommand).Assembly.GetExportedTypes()
                           .Where(x => typeof(CommandLineCommand).IsAssignableFrom(x) &&
                                       x.Namespace == typeof(CommandLineCommand).Namespace &&
                                       !x.IsAbstract));
            }
        }

        public bool Debugger { get; protected set; }

        protected string SetupPlantUml(string jarPath)
        {
            if (jarPath.IsNullOrEmpty())
                jarPath = PlantUmlJarDownloader.Download(ConsoleEnvironment.ExecutionDirectory);

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
    }
}