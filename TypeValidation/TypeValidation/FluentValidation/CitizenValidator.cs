using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeValidation.FluentValidation
{
    public class CitizenValidator : AbstractValidator<Citizen>
    {
        public CitizenValidator()
        {
            RuleFor(c => c.Age).GreaterThan(15).WithMessage("Age must be sixteen or greater.");
            RuleFor(c => c.BirtDate).Must(BeGreaterThanFiveTeenYearsOld).WithMessage("The birthdate has to be a minimum of sixteen years old");
            RuleFor(c => c.Name).NotEmpty().WithMessage("Name Cant be null or empty.");
            RuleFor(c => c.Name).Length(3, 50).WithMessage("Name has to have between three and fifty.");
        }

        bool BeGreaterThanFiveTeenYearsOld(DateTime dateToValidate)
        {

            if (dateToValidate == default(DateTime)) return false;

            DateTime zeroTime = new DateTime(1, 1, 1);
            var currentTime = DateTime.Now;
            if (currentTime <= dateToValidate) return false;
            TimeSpan span = currentTime - dateToValidate ;
            return ((zeroTime + span).Year - 1) >= 16;
        }
    }
}
