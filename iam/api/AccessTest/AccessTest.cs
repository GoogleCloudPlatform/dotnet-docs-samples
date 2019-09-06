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
        private bool ContainsMemberOne = false;
        private bool ContainsMemberTwo = false;

        public AccessTest()
        {
            _project = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            _role1 = "roles/viewer";
            _role2 = "roles/editor";
            _member1 = "user:yaraarryn.677500@gmail.com";
            _member2 = "user:obaraclegane.724498@gmail.com";
            _member3 = "user:yaraarryn.677500@gmail.com";
        }

        [Fact]
        public void TestAccess()
        {
            // Test GetPolicy
            var policy = AccessManager.GetPolicy(_project);

            // Test AddBinding by adding _member1 to _role1
            policy = AccessManager.AddBinding(policy, _role1, _member1);
            
            ContainsMemberOne = AccessManager.TestBinding(policy, _role1, _member1);
            if(!ContainsMemberOne){
                throw new Exception("Failed to add member 1");
            }

            // Test AddMember by adding _member2 to _role1
            policy = AccessManager.AddMember(policy, _role1, _member2);

            ContainsMemberTwo = AccessManager.TestBinding(policy, _role1, _member2);
            if(!ContainsMemberTwo){
                throw new Exception("Failed to add member 2");
            }

            // Test RemoveMember where role binding doesn't exist (_member1 from _role2)
            policy = AccessManager.RemoveMember(policy, _role2, _member1);

            // Test RemoveMember where member doesn't exist (_member3 from _role1)
            policy = AccessManager.RemoveMember(policy, _role1, _member3);

            // Test RemoveMember by removing _member1 from _role1
            policy = AccessManager.RemoveMember(policy, _role1, _member1);

            ContainsMemberOne = AccessManager.TestBinding(policy, _role1, _member1);
            if(ContainsMemberOne){
                throw new Exception("Failed to remove member 1");
            }

            // Test RemoveMember when removing last member from binding (_member2 from _role1)
            policy = AccessManager.RemoveMember(policy, _role1, _member2);

            ContainsMemberTwo = AccessManager.TestBinding(policy, _role2, _member2);
            if(ContainsMemberTwo)
            {
                throw new Exception("Failed to remove member 2");
            }

            // Test SetPolicy
            policy = AccessManager.SetPolicy(_project, policy);
        }
    }
}
