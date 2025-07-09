using Microsoft.AspNetCore.Mvc.Filters;
using OfficeNet.Domain.Entities;
using OfficeNet.Service;
using System.Security.Claims;

namespace OfficeNet.Filters
{
    public class SetUserContextFilter : IAsyncActionFilter
    {
        public readonly ICurrentUserService _currentUserService;

        public SetUserContextFilter(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Extract the user ID from the JWT token
            var userId = _currentUserService.GetUserId();

            if (userId != 0)
            {
                var model = context.ActionArguments.Values.FirstOrDefault();

                if (model is SurveyDetails surveyDetails)
                {
                    surveyDetails.CreatedBy = userId;
                    surveyDetails.CreatedOn = DateTime.Now;
                    surveyDetails.ModifiedBy = userId;
                    surveyDetails.ModifiedOn = DateTime.Now;
                }
            }
            await next();
        }
    }
}
