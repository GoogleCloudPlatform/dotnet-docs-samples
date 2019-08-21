// Copyright 2019 Google Inc.
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

using System;
using Xunit;

namespace GoogleCloudSamples
{
    public class AccessTest
    {
        private readonly string _project;
        private readonly string _role1;
        private readonly string _role2;
        private readonly string _member1;
        private readonly string _member2; 
        private readonly string _member3;

         public AccessTest(){
            _project = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            _role1 = "roles/viewer";
            _role2 = "roles/editor";
            _member1 = "user:user1@example.com";
            _member2 = "user:user2@example.com";
            _member3 = "user:user3@example.com";
        }

        [Fact]
        public void TestAccess()
        {
            // Test GetPolicy
            var policy = AccessManager.GetPolicy(_project);

            // Test AddBinding
            policy = AccessManager.AddBinding(policy, _role1, _member1);

            // Test AddMember
            policy = AccessManager.AddMember(policy, _role1, _member2);

            // Test RemoveMember where role binding doesn't exist
            policy = AccessManager.RemoveMember(policy, _role2, _member1);

            // Test RemoveMember where member doesn't exist
            policy = AccessManager.RemoveMember(policy, _role1, _member3);

            // Test RemoveMember
            policy = AccessManager.RemoveMember(policy, _role1, _member1);

            // Test RemoveMember when removing last member from binding
            policy = AccessManager.RemoveMember(policy, _role1, _member2);

            // Test SetPolicy
            policy = AccessManager.SetPolicy(_project, policy);
        }
    }
}
