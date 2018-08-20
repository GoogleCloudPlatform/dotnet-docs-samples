/*
 * Copyright (c) 2017 Google Inc.
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
using System.Data;
using System.Data.Common;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using CloudSql.ViewModels;

namespace CloudSql.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbConnection _connection;

        public HomeController(DbConnection connection)
        {
            this._connection = connection;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // [START gae_flex_mysql_app]
            // Insert a visit into the database:
            using (var insertVisitCommand = _connection.CreateCommand())
            {
                insertVisitCommand.CommandText =
                    @"INSERT INTO visits (user_ip) values (@user_ip)";
                var userIp = insertVisitCommand.CreateParameter();
                userIp.ParameterName = "@user_ip";
                userIp.DbType = DbType.String;
                userIp.Value =
                    FormatAddress(HttpContext.Connection.RemoteIpAddress);
                insertVisitCommand.Parameters.Add(userIp);
                await insertVisitCommand.ExecuteNonQueryAsync();
            }

            // Look up the last 10 visits.
            using (var lookupCommand = _connection.CreateCommand())
            {
                lookupCommand.CommandText = @"
                    SELECT * FROM visits
                    ORDER BY time_stamp DESC LIMIT 10";
                List<string> lines = new List<string>();
                var reader = await lookupCommand.ExecuteReaderAsync();
                HomeModel model = new HomeModel() {
                    VisitorLog = new List<VisitorLogEntry>()
                };
                while (await reader.ReadAsync()) {
                    model.VisitorLog.Add(new VisitorLogEntry() {
                        IpAddress = reader.GetString(1),
                        TimeStamp = reader.GetDateTime(0)
                    });
                }
                return View(model);
            }
            // [END gae_flex_mysql_app]
        }

        private string FormatAddress(IPAddress address)
        {
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                var bytes = address.GetAddressBytes();
                return string.Format("{0:X2}{1:X2}:{2:X2}{3:X2}", bytes[0], bytes[1],
                    bytes[2], bytes[3]);
            }
            else if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                var bytes = address.GetAddressBytes();
                return string.Format("{0}.{1}", bytes[0], bytes[1]);
            }
            else
            {
                return "bad.address";
            }
        }
    }
}
