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
using System.Web;
using System.Web.UI.WebControls;

namespace PetShop.Web
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        private const string HEADER_PREFIX = ".NET Pet Shop :: {0}";

        /// <summary>
        /// Create page header on Page PreRender event
        /// </summary>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            ltlHeader.Text = Page.Header.Title;
            Page.Header.Title = string.Format(HEADER_PREFIX, Page.Header.Title);
        }


        /// <summary>
        /// Redirect to Search page
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            WebUtility.SearchRedirect(txtSearch.Text);
        }
    }
}
