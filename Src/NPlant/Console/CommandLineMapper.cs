using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NPlant.Exceptions;

namespace NPlant.Console
{
    public static class CommandLineMapper
    {
        public static void Map(CommandLineCommand instance, string[] arguments, IEnumerable<string> options)
        {
            var properties = new HashSet<PropertyInfo>(instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public));

            foreach (var option in options)
            {
                var optionParts = option.Split(':');

                string optionName = optionParts.First();
                string optionValue = null;

                if(optionParts.Length > 1)
                    optionValue = string.Join(":", optionParts.Where((argPart, index) => index > 0));

                if (!optionName.StartsWith("--"))
                    throw new NPlantConsoleUsageException(
                        $"Arguments are expected to be in the --foo or --foo:value or --foo:\"value\" format.  Argument '{optionName}' could not be parsed.");

                optionName = optionName.Substring(2);
                var property = properties.FirstOrDefault(x => string.Equals(x.Name, optionName, StringComparison.InvariantCultureIgnoreCase));

                if (property == null)
                    throw new NPlantConsoleUsageException($"Option '{optionName}' is not recognized.");

                if (optionValue != null)
                {
                    property.SetConvertedValue(instance, optionValue);
                    properties.Remove(property);
                }
                else
                {
                    if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                    {
                        property.SetValue(instance, true, null);
                        properties.Remove(property);
                    }
                }
            }

            foreach (var property in properties.Where(x => x.HasAttribute<ArgumentAttribute>()))
            {
                var attributes = property.GetAttributesOf<ArgumentAttribute>();

                var order = attributes[0].Order;

                if (order > arguments.Length)
                    throw new NPlantConsoleUsageException($"Command argument requirements could not be satisfied - Argument in position {order} was not found.");

                property.SetConvertedValue(instance, arguments[order - 1]);
            }
        }
    }
}