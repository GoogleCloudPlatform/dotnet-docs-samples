// Program.cs
using System;

namespace ModelArmor.Samples
{
    /// <summary>
    /// Main program class that executes the QuickStart sample.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main entry point of the application that runs the Model Armor QuickStart sample.
        /// </summary>
        /// <param name="args">Command line arguments (not used).</param>
        static void Main(string[] args)
        {
            // Create a unique template ID with dotnet- prefix and a UUID
            string uniqueTemplateId = $"dotnet-{Guid.NewGuid().ToString("N").Substring(0, 12)}";

            // Create an instance of the QuickstartSample class
            QuickstartSample sample = new QuickstartSample();

            // Execute the Quickstart method with the unique template ID
            sample.Quickstart(
                projectId: "ma-crest-data-test-2",
                locationId: "us-central1",
                templateId: uniqueTemplateId
            );

            Console.WriteLine("QuickStart sample completed successfully!");
        }
    }
}
