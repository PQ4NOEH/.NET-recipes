using System;
using System.ComponentModel.DataAnnotations;

namespace TypeValidation.DataAnotations
{
    public class Citizen
    {
        [Range(54, int.MaxValue)]
        public int Age
        {
            get;
            set;
        }

        [Required]
        [MinYearsValidation(16)]
        public DateTime BirtDate
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength=3)]
        public string Name
        {
            get;
            set;
        }
    }
}
