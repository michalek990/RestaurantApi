using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace ResteurantApi.Authorization
{
    public class MinimumAgeRequiermantHandler : AuthorizationHandler<MinimumAgeRequiermant>
    {
        private readonly ILogger<MinimumAgeRequiermant> _logger;
        public MinimumAgeRequiermantHandler(ILogger<MinimumAgeRequiermant> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequiermant requirement)
        {
           var dateOfBirth= DateTime.Parse(context.User.FindFirst(c => c.Type == "DateOfBirth").Value);

           var userEmail = context.User.FindFirst(c => c.Type == ClaimTypes.Name).Value;

           _logger.LogInformation($"User {userEmail} with date birth [{dateOfBirth}]");

           if (dateOfBirth.AddYears(requirement.MinimumAge) <= DateTime.Today)
           {
               _logger.LogInformation("Authorization succedded");
               context.Succeed(requirement);
           }
           _logger.LogInformation("Authorization failed");
            return Task.CompletedTask;
        }
    }
}
