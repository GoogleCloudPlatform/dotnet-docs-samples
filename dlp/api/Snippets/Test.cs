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
using Xunit;

public class DlpSnippetsTest
{
    readonly string _projectId =
        Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

    [Fact]
    public void TestDeidMask()
    {
        var snippet = new DlpDeidentifyMasking();
        Assert.Equal("My SSN is *****9127.",
            snippet.DeidentiyMasking(_projectId));
    }

    [Fact]
    public void InspectFile()
    {
        var snippet = new DlpInspectFile();
        var findings =
            snippet.InspectFile(_projectId, "testdata/phonenumber.png");
        Assert.Equal("555-1212", findings.First().Quote);
    }
}
