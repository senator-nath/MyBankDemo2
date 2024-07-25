using FluentValidation;
using MyBankApp.Domain.Dto.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Application.validator
{
    public class UserRequestValidator : AbstractValidator<UserRequestDto>
    {
        public UserRequestValidator()
        {
            //RuleFor(x => x.FirstName)
            //.NotEmpty().WithMessage("First name is required.")
            //.Length(1, 50).WithMessage("First name must be between 1 and 50 characters.");

            //RuleFor(x => x.LastName)
            //    .NotEmpty().WithMessage("Last name is required.")
            //    .Length(1, 50).WithMessage("Last name must be between 1 and 50 characters.");

            //RuleFor(x => x.MiddleName)
            //    .MaximumLength(50).WithMessage("Middle name can be up to 50 characters long.");

            //RuleFor(x => x.UserName)
            //    .NotEmpty().WithMessage("Username is required.")
            //    .Length(3, 20).WithMessage("Username must be between 3 and 20 characters.")
            //    .Matches(@"^[a-zA-Z0-9_]*$").WithMessage("Username can only contain letters, numbers, and underscores.");

            //RuleFor(x => x.Email)
            //    .NotEmpty().WithMessage("Email is required.")
            //    .EmailAddress().WithMessage("Invalid email format.");

            //RuleFor(x => x.Password)
            //    .NotEmpty().WithMessage("Password is required.")
            //    .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            //RuleFor(x => x.ConfirmPassword)
            //    .NotEmpty().WithMessage("Confirm password is required.")
            //    .Equal(x => x.Password).WithMessage("Passwords must match.");

            //RuleFor(x => x.PhoneNumber)
            //    .Matches(@"^\+?[1-9]\d{11}$").WithMessage("Invalid phone number format.");

            //RuleFor(x => x.Age)
            //    .GreaterThan(0).WithMessage("Age must be a positive integer.");

            //RuleFor(x => x.Dob)
            //    .NotEmpty().WithMessage("Date of birth is required.")
            //    .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past.");

            //RuleFor(x => x.LGAId)
            //    .GreaterThan(0).WithMessage("LGA ID must be a positive integer.");

            //RuleFor(x => x.StateId)
            //    .GreaterThan(0).WithMessage("State ID must be a positive integer.");

            //RuleFor(x => x.Bvn)
            //    .Matches(@"^\d{11}$")
            //    .WithMessage("BVN must be exactly 11 digits long and contain only numbers");


            //RuleFor(x => x.NIN)
            //    .Matches(@"^\d{11}$")
            //    .WithMessage("NIN number must be exactly 11 digits long and contain only numbers");

            //RuleFor(x => x.LandMark)
            //    .MaximumLength(100).WithMessage("Landmark can be up to 100 characters long.");

            //RuleFor(x => x.Title)
            //    .MaximumLength(10).WithMessage("Title can be up to 10 characters long.");

            //RuleFor(x => x.AccountType)
            //    .NotEmpty().WithMessage("Account type is required.")
            //    .Length(1, 50).WithMessage("Account type must be between 1 and 50 characters.");

            //RuleFor(x => x.GenderId)
            //    .GreaterThan(0).WithMessage("Gender ID must be a positive integer.");

            //RuleFor(x => x.Gender)
            //    .IsInEnum().WithMessage("Invalid gender value.");
        }
    }
}
