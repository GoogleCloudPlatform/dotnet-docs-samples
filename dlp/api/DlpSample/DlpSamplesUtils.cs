// Copyright 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Cloud.Dlp.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GoogleCloudSamples
{
    /// <summary>
    /// Helper functions used by multiple Dlp Sample classes.
    /// </summary>
    class DlpSamplesUtils
    {
        /// <summary>
        /// Split and parse a string representation of several identifying fields.
        /// </summary>
        /// <param name="identifyingFields">Comma (default)-separated list of identifying fields to split.</param>
        /// <returns>IEnumerable of FieldId items.</returns>
        public static IEnumerable<FieldId> ParseIdentifyingFields(string identifyingFields, char separator = ',')
        {
            return identifyingFields.Split(',').Select(str =>
            {
                try
                {
                    return new FieldId { Name = str };
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to parse FieldId {str}: {e}");
                    return null;
                }
            }).Where(it => it != null);
        }

        /// <summary>
        /// Split and parse a string representation of several InfoTypes.
        /// </summary>
        /// <param name="infoTypesStr">Comma (default)-separated list of infoTypes to split.</param>
        /// <returns>IEnumerable of InfoType items.</returns>
        public static IEnumerable<InfoType> ParseInfoTypes(string infoTypesStr, char separator = ',')
        {
            return infoTypesStr.Split(',').Select(str =>
            {
                try
                {
                    return new InfoType { Name = str };
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to parse infoType {str}: {e}");
                    return null;
                }
            }).Where(it => it != null);
        }

        /// <summary>
        /// Split and parse a string representation of several quasi-identifiers.
        /// </summary>
        /// <param name="quasiIdsStr">Comma (default)-separated list of quasi-identifiers to split.</param>
        /// <returns>IEnumerable of FieldId items.</returns>
        public static IEnumerable<FieldId> ParseQuasiIds(string quasiIdsStr, char separator = ',')
        {
            return quasiIdsStr.Split(',').Select(str =>
            {
                try
                {
                    return new FieldId { Name = str };
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to parse quasi-id {str}: {e}");
                    return null;
                }
            }).Where(it => it != null);
        }
        /// <summary>
        /// Unpacks a protobuf value into a normal string
        /// </summary>
        /// <returns>The unpacked value as a string</returns>
        /// <param name="protoValue">Proto value.</param>
        public static string UnpackValue(Value protoValue) {
            Dictionary<string, object> jsonValue = JsonConvert.DeserializeObject<Dictionary<string, object>>(protoValue.ToString());
            return jsonValue.Values.ElementAt(0).ToString();
        }
    }
}
