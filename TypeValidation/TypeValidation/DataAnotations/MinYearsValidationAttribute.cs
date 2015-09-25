using System;
using System.ComponentModel.DataAnnotations;

namespace TypeValidation.DataAnotations
{
    public class MinYearsValidationAttribute : ValidationAttribute
    {
        readonly uint _minimumYears;
        public MinYearsValidationAttribute(uint minimumYears)
        {
            _minimumYears = minimumYears;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            var dateToValidate = (DateTime)value;
            if (dateToValidate == default(DateTime)) return new ValidationResult(string.Format("The {0} is not sixteen years before.", validationContext.DisplayName));

            DateTime zeroTime = new DateTime(1, 1, 1);
            var currentTime = DateTime.Now;
            
            TimeSpan span = dateToValidate - currentTime;
            if (((zeroTime + span).Year - 1) >= _minimumYears) return ValidationResult.Success;
            else return  new ValidationResult(string.Format("The {0} is not sixteen years before.", validationContext.DisplayName));
        }

    }
}
