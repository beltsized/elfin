namespace Elfin.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ElfinUsageAttribute : Attribute
    {
        public string Usage { get; init; }

        public ElfinUsageAttribute(string usage)
        {
            this.Usage = usage;
        }
    }
}