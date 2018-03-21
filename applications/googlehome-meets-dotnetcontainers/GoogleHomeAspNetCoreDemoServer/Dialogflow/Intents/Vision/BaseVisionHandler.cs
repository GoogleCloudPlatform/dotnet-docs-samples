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
using System.Collections.Generic;
using System.Linq;

namespace GoogleHomeAspNetCoreDemoServer.Dialogflow.Intents.Vision
{
    /// <summary>
    /// Base class for Vision API related intent handlers. 
    /// </summary>
    public abstract class BaseVisionHandler : BaseHandler
    {
        /// <summary>
        /// Initializes class with the conversation.
        /// </summary>
        /// <param name="conversation">Conversation</param>
        public BaseVisionHandler(Conversation conversation) : base(conversation)
        {
        }

        // Helper method to combine a list ready for DialogFlow
        protected static string CombineList(IReadOnlyList<string> col, string nonEmptyPrefix, string onEmpty) =>
            col.Count == 0 ? onEmpty :
            nonEmptyPrefix + " " + (col.Count == 1 ? col[0] :
                $"{string.Join(", ", col.Take(col.Count - 1))}, and {col.Last()}");
    }
}
