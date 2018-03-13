using Google.Cloud.Dlp.V2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoogleCloudSamples
{
    /// <summary>
    /// Helper functions used by multiple Dlp Sample classes.
    /// </summary>
    public class DlpSampleBase
    {
        /// <summary>
        /// Split and parse a string representation of several InfoTypes.
        /// </summary>
        /// <param name="infoTypesStr">Comma (default)-separated list of infoTypes to split.</param>
        /// <returns>IEnumerable of InfoType items.</returns>
        protected static IEnumerable<InfoType> ParseInfoTypes(string infoTypesStr, char separator = ',')
        {
            return infoTypesStr.Split(',').Select(str =>
            {
                try
                {
                    return InfoType.Parser.ParseJson($"{{\"name\": \"{str}\"}}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to parse infoType {str}: {e}");
                    return null;
                }
            }).Where(it => it != null);
        }
    }
}
