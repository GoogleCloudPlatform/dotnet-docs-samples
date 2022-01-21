/*
 * Copyright 2022 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Google.Protobuf.Reflection;
using System.Reflection;

// This is a TEMPORARY WORKAROUND via extension method using reflection until
// https://github.com/protocolbuffers/protobuf/issues/9425 is addressed and released.
internal static class Extensions
{
    internal static DescriptorProto ToProto(this MessageDescriptor descriptor)
    {
        PropertyInfo propertyInfo = descriptor.GetType().GetProperty("Proto", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        DescriptorProto proto = propertyInfo?.GetValue(descriptor) as DescriptorProto;
        return proto?.Clone();
    }
}