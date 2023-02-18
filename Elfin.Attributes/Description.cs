using System;

namespace Elfin.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ElfinDescriptionAttribute : Attribute
    {
        public string? Description;

        public ElfinDescriptionAttribute(string description)
        {
            this.Description = description;
        }
    }
}