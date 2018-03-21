// Copyright(c) 2018 Google Inc.
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

namespace GoogleHomeAspNetCoreDemoServer.Dialogflow
{
    /// <summary>
    /// A class level custom attribute that can be used to mark handler classes
    /// with Dialogflow intent name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class IntentAttribute : Attribute
    {
        public string Name { get; private set; }

        public IntentAttribute(string name)
        {
            Name = name;
        }
    }
}