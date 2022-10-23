using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ResteurantApi.Entities;
using ResteurantApi.Exceptions;
using ResteurantApi.Models;

namespace ResteurantApi.Services
{

    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto dto);

        string GenerateJwt(LoginDto dto);
    }
    public class AccountService : IAccountService
    {
        private readonly ResteurantDBContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authentication;
        public AccountService(ResteurantDBContext dbContext, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _authentication = authenticationSettings;
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

        public string GenerateJwt(LoginDto dto)
        {
            var user = _dbContext.User
                .Include(u=> u.Role)//pobranie informacji z bazy danych o roli usera
                .FirstOrDefault(u => u.Email == dto.Email);


            if (user == null)
            {
                throw new BadRequestException("Invalid username or password");
            }
            //sprawdzmy uzytkownika bierzemy uzytkownika, jego zahaszowane haslo z bazy danych i to co podal

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            //sprawdzamy czy podal dobre haslo
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid username or password");
            }
            //tworzymy liste claimow czyli skladowych czesci tokenu jwt
            var claims = new List<Claim>()
            {
                //id uzytkownika musi sie zawierac
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                //imie i nazwisko
                new Claim(ClaimTypes.Name, $"{user.Fristname} {user.Lastname}"),
                //jego rola
                new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
                new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd")),
                //new Claim("Nationality", user.Nationality)
            };

            //sprawdzamy czy została podana narodowosc zeby autoryzowac po niej
            if (!string.IsNullOrEmpty(user.Nationality))
            {
                claims.Add(
                    new Claim("Nationality", user.Nationality)
                    );
            }



            //generowanie tokenu
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authentication.JwtKey));
            //generowanie credenciasls do podpisania tokenu
            //zawiera klicz i algorytm jakim hasujemy token
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //obliczanie ile dni minelo
            var expires = DateTime.Now.AddDays(_authentication.JwtExpireDays);
            var token = new JwtSecurityToken(_authentication.JwtIssuer, // wlasciciel
                _authentication.JwtIssuer, //klienci
                claims, // nasze claimy jakie okreslilsmy ktore chcielismy aby znalazly sie w tokenie
                expires: expires, // dlugosc trwania tokenu 
                signingCredentials: cred); //podpis

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
