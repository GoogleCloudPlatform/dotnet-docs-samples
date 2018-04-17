/*
Copyright 2018 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using CommandLine;

namespace GoogleCloudSamples
{
    class Options
    {
        // Replace with your IAP client id listed on 
        // https://console.cloud.google.com/apis/credentials
        const string IAP_CLIENT_ID = "YOUR-IAP-CLIENT-ID";

        private string _credentialsPath;

        [Option('j', "credentials",
            HelpText = "Path to your service account credentials .json file.")]
        public string CredentialsPath
        {
            get
            {
                string path = _credentialsPath ??
                    Environment.GetEnvironmentVariable(
                        "GOOGLE_APPLICATION_CREDENTIALS");
                if (string.IsNullOrWhiteSpace(path))
                {
                    throw new Exception("Missing option: '-j'");
                }
                return path;
            }
            set { _credentialsPath = value; }
        }

        [Option('u', "uri", Required = true, HelpText = "The URI to fetch.")]
        public string Uri { get; set; }

        [Option('c', "iapcid", HelpText =
            "Your IAP client id listed on https://console.cloud.google.com/apis/credentials")]
        public string IapClientId { get; set; } = IAP_CLIENT_ID;
    }

    public class IAPClientMain
    {
        public static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts => Console.WriteLine(
                    IAPClient.InvokeRequest(opts.IapClientId, opts.CredentialsPath, opts.Uri)));
        }
    }
}
