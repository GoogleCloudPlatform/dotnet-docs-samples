using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace grs_product.Attributes
{
	public static class ExampleAttributeHelper
	{
		/// <summary>
		/// Searching for all static methods in current assembly without parameters and marked with ExampleAttribute
		/// Pass empty string to exampleName if you want all examples
		/// </summary>
		public static IEnumerable<MethodInfo> GetExamples(string exampleName)
		{
			return Assembly.GetExecutingAssembly().GetTypes()
				.SelectMany(t => t.GetMethods())
				.Where(m => m.GetCustomAttributes(typeof(ExampleAttribute), false).Length > 0)
				.Where(m => m.IsStatic && m.GetParameters().Length == 0)
				.Where(m =>
					{
						if (string.IsNullOrEmpty(exampleName))
							return true;
						var attrName = m.GetCustomAttribute<ExampleAttribute>().Name;
						if (string.IsNullOrEmpty(attrName))
							attrName = m.DeclaringType.Name;
						return attrName == exampleName;
					})
				.ToArray();
		}

		/// <summary>
		/// Execute example
		/// </summary>
		public static void ExecuteExampleMethod(MethodInfo method)
		{
			method.Invoke(null, null);
		}
	}
}