using System;

namespace RetailSearch.Samples.Attributes
{
    /// <summary>
    /// The example attribute for sample methods.
    /// </summary>
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