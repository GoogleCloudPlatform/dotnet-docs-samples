// Copyright 2020 Google Inc.
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

// [START spanner_query_with_array_parameter]
// [START spanner_query_with_bool_parameter]
// [START spanner_query_with_bytes_parameter]
// [START spanner_query_with_date_parameter]
// [START spanner_query_with_float_parameter]
// [START spanner_query_with_int_parameter]
// [START spanner_query_with_string_parameter]
// [START spanner_query_with_timestamp_parameter]
// [START spanner_insert_datatypes_data]

public class Venue
{
    public int VenueId { get; set; }
    public string VenueName { get; set; }
    public byte[] VenueInfo { get; set; }
    public int Capacity { get; set; }
    public System.Collections.Generic.List<System.DateTime> AvailableDates { get; set; }
    public System.DateTime LastContactDate { get; set; }
    public System.DateTime LastUpdateTime { get; set; }
    public bool OutdoorVenue { get; set; }
    public float PopularityScore { get; set; }
}
// [END spanner_insert_datatypes_data]
// [END spanner_query_with_timestamp_parameter]
// [END spanner_query_with_string_parameter]
// [END spanner_query_with_int_parameter]
// [END spanner_query_with_float_parameter]
// [END spanner_query_with_date_parameter]
// [END spanner_query_with_bytes_parameter]
// [END spanner_query_with_bool_parameter]
// [END spanner_query_with_array_parameter]
