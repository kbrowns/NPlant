using System;
using System.Diagnostics;
using System.Linq;
using NPlant.Exceptions;
using Con=System.Console;

namespace NPlant.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            CommandLineCommand command = null;

            try
            {
                CommandLineModel model = CommandLineModel.Parse(args);

                if (model.Debugger)
                {
                    Debugger.Launch();
                    Debugger.Break();
                }

                command = model.CreateCommand();
                CommandLineMapper.Map(command, model.Arguments.ToArray(), model.Options);

                if (command.Help)
                {
                    foreach (var line in command.Usage())
                    {
                        Con.WriteLine(line);
                    }

                    return 0;
                }
                else
                {
                    return command.Run();
                }

            }
            catch (NPlantConsoleUsageException usageException)
            {
                Con.WriteLine("Usage Error:");
                Con.WriteLine(usageException.Message);
                Con.WriteLine();

                if (command == null)
                {
                    Con.WriteLine("Available command:");

                    foreach(var commandType in CommandLineCommand.AvailableCommandTypes)
                    {
                        var parts = commandType.Name.SplitOnPascalCasing();
                        
                        if (string.Equals(parts[parts.Length -1], "Command", StringComparison.CurrentCultureIgnoreCase) )
                            parts[parts.Length - 1] = "";

                        string commandName = string.Join(" ", parts);

                        Con.WriteLine($"NPlant.Console.exe {commandName.ToLower()}");
                    }
                }
                else
                {
                    foreach (var line in command.Usage())
                    {
                        Con.WriteLine(line);
                    }
                }
            }
            catch (Exception consoleException)
            {
                Con.WriteLine("Fatal Error:");
                Con.WriteLine(consoleException);
            }

            return 1;
        }
    }
}
