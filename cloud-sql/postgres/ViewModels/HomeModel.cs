/*
 * Copyright (c) 2019 Google LLC.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using System;
using System.Collections.Generic;
using System.Data.Common;

namespace CloudSql.ViewModels {
    public class VisitorLogEntry {
        public string IpAddress { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class VoteEntry {
        public string Candidate { get; set; }
        public DateTime TimeCast { get; set; }
    }

    public class HomeModel {
        public List<VisitorLogEntry> VisitorLog { get; set; }
        public List<VoteEntry> VoteEntry { get; set; }
        public int SpaceCount { get; set; }
        public int TabCount { get; set; }
    }    
}
