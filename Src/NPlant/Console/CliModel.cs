using System;
using System.Collections.Generic;
using System.Linq;
using NPlant.Core;
using NPlant.Exceptions;

namespace NPlant.Console
{
    public class CliModel
    {
        private List<string> _arguments = new List<string>();
        private readonly List<string> _options = new List<string>();

        private CliModel()
        {

        }

        public static CliModel Parse(string[] args)
        {
            var model = new CliModel();

            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    model._options.Add(arg);
                }
                else
                {
                    model._arguments.Add(arg);
                }
            }

            return model;
        }

        public IEnumerable<string> Arguments => _arguments;

        public CliCommand CreateCommand()
        {
            var matchedType = FindCommandType(_arguments.ToArray(), out var commandArgs);
            _arguments = new List<string>();
            _arguments.AddRange(commandArgs);

            return matchedType.InstantiateAs<CliCommand>();
        }

        public static Type FindCommandType(string[] arguments, out string[] commandArguments)
        {
            var possibilities = new Stack<Tuple<string, ArraySegment<string>>>();

            for (var index = 0; index < arguments.Length; index++)
            {
                int position = index + 1;

                ArraySegment<string> commandName = new ArraySegment<string>(arguments, 0, position);
                ArraySegment<string> commandArgs =
                    new ArraySegment<string>(arguments, position, arguments.Length - position);
                possibilities.Push(new Tuple<string, ArraySegment<string>>(string.Join("", commandName), commandArgs));
            }

            var types = typeof(CliCommand).Assembly.GetExportedTypes();

            while (possibilities.Count > 0)
            {
                var candidate = possibilities.Pop();
                var expected = $"{typeof(CliCommand).Namespace}.{candidate.Item1}Command";

                var matchedType = types.FirstOrDefault(x =>
                    string.Equals(x.FullName, expected, StringComparison.InvariantCultureIgnoreCase));

                if (matchedType != null)
                {
                    commandArguments = candidate.Item2.ToArray();
                    return matchedType;
                }
            }

            throw new NPlantConsoleUsageException("Failed to find a the command implementation");
        }

        public bool Debugger => _options.Contains("--debugger");
        public IEnumerable<string> Options => _options;
    }
}