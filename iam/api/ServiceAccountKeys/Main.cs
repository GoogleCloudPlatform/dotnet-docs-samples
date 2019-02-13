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

using CommandLine;

public partial class ServiceAccountKeys
{
    public static int Main(string[] args)
    {
        return Parser.Default.ParseArguments<
            CreateKeyOptions,
            ListKeyOptions,
            DeleteKeyOptions
            >(args).MapResult(
            (CreateKeyOptions x) =>
            {
                CreateKey(x.ServiceAccountEmail);
                return 0;
            },
            (ListKeyOptions x) =>
            {
                ListKeys(x.ServiceAccountEmail);
                return 0;
            },
            (DeleteKeyOptions x) =>
            {
                DeleteKey(x.FullKeyName);
                return 0;
            },
            err => 1);
    }
}