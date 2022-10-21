using ResteurantApi.Entities;
using ResteurantApi.Models;

namespace ResteurantApi.Services
{

    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto dto);
    }
    public class AccountService : IAccountService
    {
        private readonly ResteurantDBContext _dbContext;
        public AccountService(ResteurantDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void RegisterUser(RegisterUserDto dto)
        {
            var newUser = new User()
            {
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Nationality = dto.Nationality,
                RoleId = dto.RoleId
            };

            _dbContext.User.Add(newUser);
            _dbContext.SaveChanges();

        }
    }
}
