// Copyright 2018 Google Inc.
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

using Google;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace GoogleCloudSamples
{
    public class CustomRolesTest
    {
        private readonly string _project;
        private readonly string _name;
        private readonly string _title;
        private readonly string _description;
        private readonly List<string> _permissions;
        private readonly string _stage;

        public CustomRolesTest()
        {
            _project = Environment.GetEnvironmentVariable(
                "GOOGLE_PROJECT_ID");
            _name = "csharpTestCustomRole" + new Random().Next();
            _title = "C# Test Custom Role";
            _description = "This is a C# test custom role.";
            _permissions = new List<string> { "iam.roles.get" };
            _stage = "GA";
        }

        [Fact]
        public void TestGrantableRoles()
        {
            CustomRoles.ViewGrantableRoles(
                "//cloudresourcemanager.googleapis.com/projects/" + _project);
        }

        [Fact]
        public void TestRoleInfo()
        {
            CustomRoles.QueryTestablePermissions(
                "//cloudresourcemanager.googleapis.com/projects/" + _project);
            CustomRoles.GetRole("roles/appengine.appViewer");
        }

        [SkippableFact]
        public void TestCustomRole()
        {
            try
            {
                CustomRoles.CreateRole(_name, _project, _title, _description, _permissions, _stage);
            }
            catch (GoogleApiException ex) when (ex.HttpStatusCode == HttpStatusCode.TooManyRequests && ex.Error.Message.Contains("Maximum number of roles reached"))
            {
                Skip.If(true, "Maximum number of roles reached.");
                throw; // We should never throw here (Skip throws), but we need to make the compiler happy.
            }

            CustomRoles.ListRoles(_project);
            CustomRoles.EditRole(_name, _project, _title, "Updated C# description.", _permissions, _stage);
            CustomRoles.DeleteRole(_name, _project);
            CustomRoles.UndeleteRole(_name, _project);

            CustomRoles.DeleteRole(_name, _project);
        }
    }
}