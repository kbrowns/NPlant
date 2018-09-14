using System;

namespace NPlant.Console
{
    public class CliArgumentAttribute : Attribute
    {
        public CliArgumentAttribute(int order)
        {
            this.Order = order;
        }

        public int Order { get; private set; }

        public string[] Allowed { get; set; }
    }
}