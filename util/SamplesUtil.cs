/*
 * Copyright (c) 2015 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using System;
using System.Diagnostics;

namespace GoogleCloudSamples
{
    public class SamplesUtil
    {
        public delegate void MainFunction();

        /// <summary>
        /// Prevent console window from disappearing before the user sees it.
        /// </summary>
        /// <param name="main"></param>
        public static void InvokeMain(MainFunction main)
        {
            if (String.IsNullOrEmpty(Process.GetCurrentProcess().
                MainWindowTitle))
            {
                // Process is running in its parent's console window.  The
                // user will see output and any exceptions raised.
                main();
                return;
            }
            // Prevent console window from disappearing before user
            // sees it.
            try
            {
                main();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
            finally
            {
                {
                    Console.WriteLine("\nPress any key...");
                    Console.ReadKey();
                }
            }
        }
    }
}