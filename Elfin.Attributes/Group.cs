using System;

namespace Elfin.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ElfinGroupAttribute : Attribute
    {
        public string Name { get; init; }
        
        public ElfinGroupAttribute(string name)
        {
            this.Name = name;
        }
    }
}