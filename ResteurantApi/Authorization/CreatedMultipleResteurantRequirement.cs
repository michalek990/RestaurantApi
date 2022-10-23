using Microsoft.AspNetCore.Authorization;

namespace ResteurantApi.Authorization
{
    public class CreatedMultipleResteurantRequirement : IAuthorizationRequirement
    {
        public int NumberOfResteurant { get; }

        public CreatedMultipleResteurantRequirement(int minimNumberOfResteurant)
        {
            NumberOfResteurant = minimNumberOfResteurant;
        }
    }
}
