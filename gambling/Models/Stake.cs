using FluentValidation;
using gambling.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace gambling.Models
{
    public class Stake
    {
        public double points { get; set; }
        public int number { get; set; }

    }

    public class StakeValidator : AbstractValidator<Stake>
    {
        private readonly double _balance;
        public StakeValidator(double balance)
        {
            _balance = balance;
            RuleFor(x => x.points).NotNull().NotEmpty().LessThanOrEqualTo(_balance).WithMessage("Insufficient credits! Please try again.");
            RuleFor(x => x.number).NotEmpty().InclusiveBetween(1, 9);
            _balance = balance;
        }
    }
}
