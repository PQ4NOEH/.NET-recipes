using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeValidation.FluentValidation;
using Xunit;

namespace TypeValidation.UnitTests.FluentValidation
{
    public class CitizenSpec
    {
        [Fact]
        public void When_Age_is_under_sixteen_the_object_is_invalid()
        {
            var sut = new Citizen();
            sut.Age = new Random().Next(15);
            sut.BirtDate = DateTime.Parse("1965/10/12");
            sut.Name = "AValidName";
            var validator = new CitizenValidator();
            var results = validator.Validate(sut);

            Assert.False(results.IsValid);
            Assert.Equal(results.Errors.Count, 1);
            Assert.Contains("Age must be sixteen or greater", results.Errors.First().ErrorMessage);
            Assert.Contains("Age", results.Errors.First().PropertyName);
        }

        [Fact]
        public void When_Age_is_sixteen_the_object_is_valid()
        {
            var sut = new Citizen();
            sut.Age = 16;
            sut.BirtDate = DateTime.Parse("1965/10/12");
            sut.Name = "AValidName";
            var validator = new CitizenValidator();
            var results = validator.Validate(sut);

            Assert.True(results.IsValid);
        }

        [Fact]
        public void When_Age_is_over_sixteen_the_object_is_valid()
        {
            var sut = new Citizen();
            sut.Age = new Random().Next(16, int.MaxValue);
            sut.BirtDate = DateTime.Parse("1965/10/12");
            sut.Name = "AValidName";
            var validator = new CitizenValidator();
            var results = validator.Validate(sut);

            Assert.True(results.IsValid);
        }

        

        [Fact]
        public void When_BirthDate_is_not_more_than_fiveteen_years_before_now_the_object_is_invalid()
        {
            var sut = new Citizen();
            sut.Age = 16;
            sut.BirtDate = DateTime.Today.AddYears(-new Random().Next(0, 15));
            sut.Name = "AValidName";
            var validator = new CitizenValidator();
            var results = validator.Validate(sut);

            Assert.False(results.IsValid);
            Assert.Equal(results.Errors.Count, 1);
            Assert.Contains("minimum of sixteen years old", results.Errors.First().ErrorMessage);
            Assert.Contains("BirtDate", results.Errors.First().PropertyName);
        }
        [Fact]
        public void When_BirthDate_is_sixteen_years_before_now_the_object_is_valid()
        {
            var sut = new Citizen();
            sut.Age = 16;
            sut.BirtDate = DateTime.Today.AddYears(-16);
            sut.Name = "AValidName";
            var validator = new CitizenValidator();
            var results = validator.Validate(sut);

            Assert.True(results.IsValid);
        }
        [Fact]
        public void When_BirthDate_is_more_than_sixteen_years_before_now_the_object_is_valid()
        {
            var sut = new Citizen();
            sut.Age = 16;
            sut.BirtDate = DateTime.Today.AddYears(-new Random().Next(17, 200));
            sut.Name = "AValidName";
            var validator = new CitizenValidator();
            var results = validator.Validate(sut);

            Assert.True(results.IsValid);
        }

        [Fact]
        public void If_the_name_is_null_the_object_is_invalid()
        {
            var sut = new Citizen();
            sut.Age = 20;
            sut.BirtDate = DateTime.Parse("1965/10/12");
            sut.Name = null;
            var validator = new CitizenValidator();
            var results = validator.Validate(sut);

            Assert.False(results.IsValid);
            Assert.Equal(results.Errors.Count, 1);
            Assert.Contains("Name Cant be null or empty", results.Errors.First().ErrorMessage);
            Assert.Contains("Name", results.Errors.First().PropertyName);
        }
        [Fact]
        public void If_the_name_is_empty_the_object_is_invalid()
        {
            var sut = new Citizen();
            sut.Age = 20;
            sut.BirtDate = DateTime.Parse("1965/10/12");
            sut.Name = "";
            var validator = new CitizenValidator();
            var results = validator.Validate(sut);

            Assert.False(results.IsValid);
            Assert.Equal(results.Errors.Count, 2);
            Assert.Contains("Name Cant be null or empty", results.Errors.First().ErrorMessage);
            Assert.Contains("Name has to have between three and fifty", results.Errors.Last().ErrorMessage);
            Assert.Contains("Name", results.Errors.First().PropertyName);
            Assert.Contains("Name", results.Errors.Last().PropertyName);
        }
        [Fact]
        public void If_the_name_is_whiteSpaces_the_object_is_invalid()
        {
            var sut = new Citizen();
            sut.Age = 20;
            sut.BirtDate = DateTime.Parse("1965/10/12");
            sut.Name = "      ";
            var validator = new CitizenValidator();
            var results = validator.Validate(sut);

            Assert.False(results.IsValid);
            Assert.Equal(results.Errors.Count, 1);
            Assert.Contains("Name Cant be null or empty", results.Errors.First().ErrorMessage);
            Assert.Contains("Name", results.Errors.First().PropertyName);
        }
        [Fact]
        public void If_the_name_is_less_than_three_characters_length_the_object_is_invalid()
        {
            var sut = new Citizen();
            sut.Age = 20;
            sut.BirtDate = DateTime.Parse("1965/10/12");
            sut.Name = "we";
            var validator = new CitizenValidator();
            var results = validator.Validate(sut);

            Assert.False(results.IsValid);
            Assert.Equal(results.Errors.Count, 1);
            Assert.Contains("Name has to have between three and fifty", results.Errors.Last().ErrorMessage);
            Assert.Contains("Name", results.Errors.Last().PropertyName);
        }
        [Fact]
        public void If_the_name_is_more_than_fifty_characters_length_the_object_is_invalid()
        {
            var sut = new Citizen();
            sut.Age = 20;
            sut.BirtDate = DateTime.Parse("1965/10/12");
            sut.Name = "WeweweweweWeweweweweWeweweweweWeweweweweWewewewewes";
            var validator = new CitizenValidator();
            var results = validator.Validate(sut);

            Assert.False(results.IsValid);
            Assert.Equal(results.Errors.Count, 1);
            Assert.Contains("Name has to have between three and fifty", results.Errors.Last().ErrorMessage);
            Assert.Contains("Name", results.Errors.Last().PropertyName);
        }

        [Fact]
        public void If_the_name_is_not_null_whiteSpace_or_empty_and_has_a_length_between_three_and_fifty_characters_the_object_is_valid()
        {
            var sut = new Citizen();
            sut.Age = 20;
            sut.BirtDate = DateTime.Parse("1965/10/12");
            sut.Name = "b difbjf";
            var validator = new CitizenValidator();
            var results = validator.Validate(sut);

            Assert.True(results.IsValid);
            
        }
    }
}
