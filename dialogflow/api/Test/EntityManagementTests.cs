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
using Xunit;

namespace GoogleCloudSamples
{
    public class EntityManagementTests : DialogflowTest
    {
        readonly string EntityTypeDisplayName = TestUtil.RandomName();

        public EntityManagementTests()
        {
            // Create EntityType for each test.
            // Required as prerequisite for Entity management.
            CreateEntityType(EntityTypeDisplayName);
        }

        [Fact(Skip = "Not implemented")]
        void TestCreate()
        {

        }

        [Fact(Skip = "Not implemented")]
        void TestList()
        {

        }

        [Fact(Skip = "Not Implemented")]
        void TestDelete()
        {

        }
    }
}
