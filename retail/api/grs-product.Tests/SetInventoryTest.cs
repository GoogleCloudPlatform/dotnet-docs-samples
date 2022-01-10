// Copyright 2021 Google Inc. All Rights Reserved.
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

using Google.Cloud.Retail.V2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace grs_product.Tests
{
    [TestClass]
    public class SetInventoryTest
    {
        private const string ProductFolderName = "grs-product";

        private const string WindowsTerminalVarName = "ComSpec";
        private const string UnixTerminalVarName = "SHELL";
        private const string WindowsTerminalPrefix = "/c ";
        private const string UnixTerminalPrefix = "-c ";

        private static readonly string WorkingDirectory = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, ProductFolderName);

        private static readonly string CurrentOperatingSystemName = Environment.OSVersion.VersionString;
        private static readonly string CurrentTerminalVarName = CurrentOperatingSystemName.Contains("Windows") ? WindowsTerminalVarName : UnixTerminalVarName;
        private static readonly string CurrentTerminalPrefix = CurrentOperatingSystemName.Contains("Windows") ? WindowsTerminalPrefix : UnixTerminalPrefix;
        private static readonly string CurrentTerminalFile = "/bin/bash";
        private static readonly string CommandLineArguments = "-c \"dotnet run -- SetInventory\"";

        [TestMethod]
        public void TestSetInventory()
        {
            const string ExpectedCurrencyCode = "USD";
            const float ExpectedProductPrice = 30.0f;
            const float ExpectedProductOriginalPrice = 35.5f;
            const Product.Types.Availability ExpectedProductAvailability = Product.Types.Availability.InStock;

            var inventoryProduct = SetInventory.PerformSetInventoryOperation();

            Assert.AreEqual(ExpectedCurrencyCode, inventoryProduct.PriceInfo.CurrencyCode);
            Assert.AreEqual(ExpectedProductPrice, inventoryProduct.PriceInfo.Price);
            Assert.AreEqual(ExpectedProductOriginalPrice, inventoryProduct.PriceInfo.OriginalPrice);
            Assert.AreEqual(ExpectedProductAvailability, inventoryProduct.Availability);
        }

        [TestMethod]
        public void TestOutputSetInventory()
        {
            string consoleOutput = string.Empty;

            var processStartInfo = new ProcessStartInfo(CurrentTerminalFile, CommandLineArguments)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = WorkingDirectory
            };

            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;

                process.Start();

                consoleOutput = process.StandardOutput.ReadToEnd();
            }

            Assert.IsTrue(Regex.Match(consoleOutput, "(.*)Created product:(.*)").Success);
            Assert.IsTrue(Regex.Match(consoleOutput, "(.*)Created product:(.*)\"name\": \"projects/(.*)/locations/global/catalogs/default_catalog/branches/0/products/inventory_test_product_id\"(.*)", RegexOptions.Singleline).Success);
            Assert.IsTrue(Regex.Match(consoleOutput, "(.*)Created product:(.*)\"title\": \"Nest Mini\"(.*)", RegexOptions.Singleline).Success);

            Assert.IsTrue(Regex.Match(consoleOutput, "(.*)Set inventory. request:(.*)").Success);

            Assert.IsTrue(Regex.Match(consoleOutput, "(.*)Get product. response:(.*)\"fulfillmentInfo\"(.*)\"type\": \"pickup-in-store\"(.*)\"placeIds\":(.*)\"store1\"(.*)", RegexOptions.Singleline).Success);
            Assert.IsTrue(Regex.Match(consoleOutput, "(.*)Get product. response:(.*)\"fulfillmentInfo\"(.*)\"type\": \"pickup-in-store\"(.*)\"placeIds\":(.*)\"store2\"(.*)", RegexOptions.Singleline).Success);

            Assert.IsTrue(Regex.Match(consoleOutput, "(.*)Product projects/(.*)/locations/global/catalogs/default_catalog/branches/default_branch/products/inventory_test_product_id was deleted(.*)").Success);
        }
    }
}