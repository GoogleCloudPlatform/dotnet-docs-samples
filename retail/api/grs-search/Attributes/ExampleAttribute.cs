using System;

namespace grs_search.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ExampleAttribute: Attribute
    {
        public ExampleAttribute()
        {
            Name = string.Empty;
        }

        public ExampleAttribute(string name)
        {
            Name = name;
        }

        public string Name
        {
            get; init;
        }
    }
}