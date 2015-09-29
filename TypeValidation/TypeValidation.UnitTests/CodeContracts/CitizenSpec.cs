using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeValidation.CodeContract;
using Xunit;

namespace TypeValidation.UnitTests.CodeContracts
{
    public class CitizenSpec
    {
        [Fact]
        public void Setting_Age_under_sixteen_throws_exception()
        {
            var sut = new Citizen();
            var ex = Assert.Throws<ArgumentOutOfRangeException>(()=>sut.Age = new Random().Next(15));
            Assert.Contains("Age must be sixteen", ex.Message);
        }
    }
}
