using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TypeValidation.DataAnotations;
using Xunit;

namespace TypeValidation.UnitTests.DataAnotations
{
    public class CitizenSpec
    {
        bool ValidateObject(Citizen obj, ICollection<ValidationResult> validationResults)
        {
            var context = new ValidationContext(obj, serviceProvider: null, items: null);

            return Validator.TryValidateObject(obj, context, validationResults);
        }
        [Fact]
        public void When_Age_is_under_sixteen_the_object_is_invalid()
        {
            var sut = new Citizen();
            sut.Age = 12;
            sut.BirtDate = DateTime.Parse("2015/10/12");
            sut.Name = "AValidName";
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(sut);

            Assert.False(Validator.TryValidateObject(sut, context, validationResults));
            Assert.Equal(validationResults.Count, 1);
        }
    }
}
