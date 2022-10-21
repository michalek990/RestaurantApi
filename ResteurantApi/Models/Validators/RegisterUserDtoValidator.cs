using System.Linq;
using FluentValidation;
using ResteurantApi.Entities;

namespace ResteurantApi.Models.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(ResteurantDBContext dbContext)
        {

            //walidacja dla konkretnych pol
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .MinimumLength(6);

            RuleFor(x => x.ConfirmPassword)
                .Equal(e => e.Password);

            RuleFor(x => x.Email)
               //customowa walidacja sprwadzajaca czy istnieje juz uztkownik o konkretnym emailu
                .Custom((value, context) =>
                {
                    var emailInUse = dbContext.User.Any(i => i.Email == value);
                    if (emailInUse)
                    {
                        context.AddFailure("Email", "Email is taken");
                    }
                });
        }
    }
}
