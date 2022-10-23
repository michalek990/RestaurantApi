using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ResteurantApi.Entities;

namespace ResteurantApi.Authorization
{
    public class ResourceOperationRequirementHandler : AuthorizationHandler<ResourceOperationRequirement, Resteurant>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement,
            Resteurant resteurant)
        {
            //pozwalamy wszytskim uzytkownikom odczytywac dane i tworzyc nowe
            if (requirement.ResourceOperation == ResourceOperation.Create ||
                requirement.ResourceOperation == ResourceOperation.Read)
            {
                context.Succeed(requirement);
            }

            //edytowac i usuwac pozawalamy tylko temu uzytkownikowi ktory dodal dana resteuracje
            var UserID = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (resteurant.CreatedById == int.Parse(UserID))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
