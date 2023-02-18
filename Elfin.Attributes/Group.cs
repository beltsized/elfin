using System;

namespace Elfin.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ElfinGroupAttribute : Attribute
    {
        public string? Name;

        public ElfinGroupAttribute(string name)
        {
            this.Name = name;
        }
    }
}