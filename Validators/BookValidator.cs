using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Validators
{
    public class BookValidator:AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(n => n.Name).NotNull().NotEmpty().WithMessage("Name cannot be null");
            RuleFor(n => n.Name).MaximumLength(255).WithMessage("Maximum length can be 255");

            RuleFor(n => n.Price).Must(n=>((int)n)>=1).WithMessage("must be at least 1$");
        }
    }
}
