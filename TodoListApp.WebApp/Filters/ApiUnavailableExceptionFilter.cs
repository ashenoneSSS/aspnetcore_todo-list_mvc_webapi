using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TodoListApp.WebApp.Exceptions;

namespace TodoListApp.WebApp.Filters;

/// <summary>
/// Handles <see cref="ApiUnavailableException"/> and returns a friendly view.
/// </summary>
public class ApiUnavailableExceptionFilter : IExceptionFilter
{
    /// <inheritdoc />
    public void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (context.Exception is ApiUnavailableException)
        {
            var metadataProvider = context.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();
            context.Result = new ViewResult
            {
                ViewName = "ApiUnavailable",
                ViewData = new ViewDataDictionary(metadataProvider, context.ModelState)
                {
                    ["Message"] = context.Exception.Message,
                },
            };
            context.ExceptionHandled = true;
        }
    }
}
