using System;

namespace Elfin.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ElfinEventAttribute : Attribute
    {
        public string Name { get; init; }

        public ElfinEventAttribute(string name)
        {
            this.Name = name;
        }
    }
}