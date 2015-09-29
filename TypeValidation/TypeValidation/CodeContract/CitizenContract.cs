using System;
using System.Diagnostics.Contracts;

namespace TypeValidation.CodeContract
{
    [ContractClassFor(typeof(ICitizen))]
    public abstract class CitizenContract : ICitizen
    {
        public int Age
        {
            get { return default(int); }
            set
            {

                Contract.Requires<ArgumentOutOfRangeException>(value > 15, "Age must be sixteen or greater.");
            }
        }


        public DateTime BirtDate
        {
            get { return default(DateTime); }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(MoreThanFifTeenYearsOldValidator.IsValid(value), "The birthdate has to be a minimum of sixteen years old");
            }
        }

        public string Name
        {
            get { return default(string); }
            set
            {
                Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(value), "Name Cant be null or empty.");
                Contract.Requires<ArgumentOutOfRangeException>(value.Length >= 3 && value.Length <= 50, "Name has to have between three and fifty.");
            }
        }

       
    }

    public static class MoreThanFifTeenYearsOldValidator
    {
        [Pure]
        public static bool IsValid(DateTime dateToValidate)
        {

            if (dateToValidate == default(DateTime)) return false;

            DateTime zeroTime = new DateTime(1, 1, 1);
            var currentTime = DateTime.Now;
            if (currentTime <= dateToValidate) return false;
            TimeSpan span = currentTime - dateToValidate;
            return ((zeroTime + span).Year - 1) >= 16;
        }
    }
}
