// Copyright(c) 2020 Google LLC.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace GoogleCloudSamples
{
    public class GameServersTestsBase
    {
        protected string RegionName { get; private set; } = "us-central1";
        protected string RealmId { get; private set; } = "fake-realm-for-test";
        protected string GameServerClusterId { get; private set; } = "fake-cluster-for-test";

        private readonly CommandLineRunner _productSearch = new CommandLineRunner()
        {
            Main = GameServersProgram.Main,
            Command = "Game Servers"
        };

        // Keep a list of all the things created while running tests.
        private readonly List<string[]> _createCommands = new List<string[]>();

        /// <summary>
        ///  Run the command and track all cloud assets that were created.
        /// </summary>
        /// <param name="arguments">The command arguments.</param>
        public ConsoleOutput Run(params string[] arguments)
        {
            if (arguments[0].StartsWith("create_"))
            {
                _createCommands.Add(arguments);
            }
            return _productSearch.Run(arguments);
        }

        /// <summary>
        /// Delete all the things created in Run() commands.
        /// </summary>
        protected void DeleteCreations()
        {
            // Clean up everything the test created.
            List<string[]> commands = new List<string[]>(_createCommands);
            _createCommands.Clear();
            commands.Reverse();

            var exceptions = new List<Exception>();
            foreach (string[] command in commands)
            {
                command[0] = command[0].Replace("create_", "delete_");
                try
                {
                    Run(command);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }
            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        /// <summary>
        /// Add a random chunk to all the Ids used in the tests, so that
        /// multiple machines can run the same tests at the same time
        /// in the same Google Cloud project without interfering with each
        /// other.
        /// </summary>
        protected void RandomizeIds()
        {
            RealmId += TestUtil.RandomName();
            GameServerClusterId += TestUtil.RandomName();
        }
    }

    public class GameServersTests : GameServersTestsBase, IDisposable
    {
        public GameServersTests()
        {
            RandomizeIds();
        }

        public void Dispose()
        {
            DeleteCreations();
        }
    }
}
