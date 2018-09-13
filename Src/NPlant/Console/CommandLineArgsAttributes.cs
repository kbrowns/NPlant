using System;

namespace NPlant.Console
{
    public class ArgumentAttribute : Attribute
    {
        public ArgumentAttribute(int order)
        {
            this.Order = order;
        }

        public int Order { get; private set; }
    }
}