using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace PensionManagementPortal.Filters
{
    public class PensionExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly ILogger<PensionExceptionFilter> _logger;

        public PensionExceptionFilter(IModelMetadataProvider modelMetadataProvider, ILogger<PensionExceptionFilter> logger)
        {
            _modelMetadataProvider = modelMetadataProvider;
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception.Message);

            ViewDataDictionary viewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);
            viewData.Add("Message", context.Exception.Message);
            context.ExceptionHandled = true;
            context.Result = new ViewResult { ViewName = "Error", ViewData = viewData };
        }
    }
}
