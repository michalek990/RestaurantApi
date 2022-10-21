using Microsoft.AspNetCore.Identity;
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
        private readonly IPasswordHasher<User> _passwordHasher;
        public AccountService(ResteurantDBContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
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

            //hashoowanie hasła
            var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);

            newUser.PasswordHash = hashedPassword;
            _dbContext.User.Add(newUser);
            _dbContext.SaveChanges();

        }
    }
}
