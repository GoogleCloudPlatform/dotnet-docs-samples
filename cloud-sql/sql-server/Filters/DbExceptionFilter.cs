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
using System.Data.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

// DbExceptionFilterAttribute is an ExceptionFilter to 
// manage unhandled database exceptions
public class DbExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly IHostEnvironment _hostingEnvironment;
    private readonly IModelMetadataProvider _modelMetadataProvider;

    public DbExceptionFilterAttribute(
        IHostEnvironment hostingEnvironment,
        IModelMetadataProvider modelMetadataProvider)
    {
        _hostingEnvironment = hostingEnvironment;
        _modelMetadataProvider = modelMetadataProvider;
    }

    public override void OnException(ExceptionContext context)
    {
        var dbException = context.Exception as DbException;
        if (!_hostingEnvironment.IsDevelopment() || null == dbException)
        {
            // Log exceptions here from 'Staging' and 'Production'
            // hosting environments. Hosting environment is set in
            // Properties/launchSettings.json
            return;
        }
        // Hosting environment is 'Development' so display exception in View
        var result = new ViewResult {ViewName = "DbException"};
        result.ViewData = new ViewDataDictionary(_modelMetadataProvider,context.ModelState);
        result.ViewData.Add("Exception", dbException);
        // TODO: Pass additional detailed data via ViewData
        context.Result = result;
    }
}
