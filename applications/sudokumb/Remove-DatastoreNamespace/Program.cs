// Copyright (c) 2018 Google LLC.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using System.Linq;
using CommandLine;
using Google.Cloud.Datastore.V1;

namespace Remove_DatastoreNamespace
{
    class Options
    {
        [Option('p', "projectid", Required = true, HelpText = "Your Google Cloud Project ID")]
        public string ProjectId { get; set; }

        [Option('n', "namespace", Required = true, HelpText = "The namespace to delete.")]
        public string Namespace { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts => RemoveNamespace(opts));
        }

        static int RemoveNamespace(Options opts)
        {
            var datastore = DatastoreDb.Create(opts.ProjectId, opts.Namespace);
            var query = new Query()
            {
                Projection = { "__key__" }
            };
            int totalCountRemoved = 0;
            while (true)
            {
                var entities = datastore.RunQuery(query).Entities;
                if (entities.Count() == 0)
                {
                    break;
                }
                datastore.Delete(entities);
                totalCountRemoved += entities.Count();
            }
            Console.WriteLine("Removed {0} entities from {1}.",
                totalCountRemoved, opts.Namespace);
            return 0;
        }
    }
}
