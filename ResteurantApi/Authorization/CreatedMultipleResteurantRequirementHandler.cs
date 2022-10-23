using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ResteurantApi.Entities;

namespace ResteurantApi.Authorization
{
    public class CreatedMultipleResteurantRequirementHandler : AuthorizationHandler<CreatedMultipleResteurantRequirement>
    {
        private readonly ResteurantDBContext _dbContext;

   
        public CreatedMultipleResteurantRequirementHandler(ResteurantDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreatedMultipleResteurantRequirement requirement)
        {
            var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var createdCount = _dbContext
                .Resteurants
                .Count(r => r.CreatedById == userId);
            if (createdCount >= requirement.NumberOfResteurant)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
