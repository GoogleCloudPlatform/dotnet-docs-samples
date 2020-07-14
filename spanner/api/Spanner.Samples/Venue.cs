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

using System;
using System.Collections.Generic;

public class Venue
{
    public int VenueId { get; set; }
    public string VenueName { get; set; }
    public byte[] VenueInfo { get; set; }
    public int Capacity { get; set; }
    public List<DateTime> AvailableDates { get; set; }
    public DateTime LastContactDate { get; set; }
    public bool OutdoorVenue { get; set; }
    public float PopularityScore { get; set; }
}
