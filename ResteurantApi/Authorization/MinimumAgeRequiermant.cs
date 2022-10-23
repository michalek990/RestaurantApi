using Microsoft.AspNetCore.Authorization;
using ResteurantApi.Services;

namespace ResteurantApi.Authorization
{
    public class MinimumAgeRequiermant : IAuthorizationRequirement
    {
        public int MinimumAge { get; }

        public MinimumAgeRequiermant(int minimumAge)
        {
            MinimumAge = minimumAge;
        }
    }
}
