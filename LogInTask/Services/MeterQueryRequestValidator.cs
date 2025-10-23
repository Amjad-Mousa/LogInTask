using FluentValidation;
using LogInTask.Models;
using System.Text.RegularExpressions;

namespace ElectricMeterApp.Validators
{
    public class MeterQueryRequestValidator : AbstractValidator<MeterQueryRequest>
    {
        public MeterQueryRequestValidator()
        {
            RuleFor(x => x.MeterNo)
                .NotEmpty().WithMessage("Meter number is required.")
                .Must(BeDigitsOnly).WithMessage("Meter number must contain digits only.")
                .Must(BeValidLength).WithMessage("Meter number must be exactly 11 or 13 digits.");

            RuleFor(x => x.Amount).NotEmpty()
                .WithMessage("Amount is required.")
                .GreaterThanOrEqualTo(20)
                .LessThanOrEqualTo(500)
                .WithMessage("Amount must be between 20 and 500."); 

        }

        private bool BeDigitsOnly(string? s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            return Regex.IsMatch(s, @"^\d+$");
        }

        private bool BeValidLength(string? s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            return s.Length == 11 || s.Length == 13;
        }
    }
}
