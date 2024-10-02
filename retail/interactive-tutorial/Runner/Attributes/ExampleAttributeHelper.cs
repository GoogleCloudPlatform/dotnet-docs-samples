// Copyright 2022 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Runner.Attributes
{
	/// <summary>
	/// Helper for identifying sample methods from the sample classes by name.
	/// </summary>
	public static class ExampleAttributeHelper
	{
		/// <summary>
		/// Searching for all static methods in current assembly without parameters and marked with ExampleAttribute.
		/// Pass empty string to exampleName if you want all examples.
		/// </summary>
		public static IEnumerable<MethodInfo> GetExamples(string exampleName)
		{
			return Assembly.GetEntryAssembly().GetTypes()
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
		/// Execute example.
		/// </summary>
		public static void ExecuteExampleMethod(MethodInfo method)
		{
			method.Invoke(null, null);
		}
	}
}
