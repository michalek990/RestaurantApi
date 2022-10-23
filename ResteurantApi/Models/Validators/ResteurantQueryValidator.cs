using System.Data;
using System.Linq;
using FluentValidation;
using ResteurantApi.Entities;

namespace ResteurantApi.Models.Validators
{
    public class ResteurantQueryValidator : AbstractValidator<ResteurantQuery>
    {
        private int[] allowedPageSizes = new[] { 5, 10, 15 };
        private string[] allowedSortByColumnNames = {nameof(Resteurant.Name), nameof(Resteurant.Description), nameof(Resteurant.Category)};
public ResteurantQueryValidator()
        {
            //walidacja pozwalajaca nam wyswietlac na stronie jedynie 5 10 lub 15 wynikow i musi byc wieksza niz 1 albo rowna 1
            RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(r => r.PageSize).Custom((value, context) =>
            {
                if (!allowedPageSizes.Contains(value))
                {
                    context.AddFailure("PageSize", $"Page Size must be in [{string.Join(",", allowedPageSizes)}]");
                }
            });

            RuleFor(r => r.SortBy)
                .Must(value => string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value))
                .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");

        }
    }
}
