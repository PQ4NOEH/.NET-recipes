using Davalor.MomProxy.Domain.Configuration;
using System;
using Xunit;

namespace Davalor.MomProxy.Domain.UnitTests.Configuration
{
    public class MinuteSpec
    {
        [Fact]
        public void If_a_value_under_zero_is_given_throws_an_exception()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Minute(-1));
            Assert.Contains("from 0", ex.Message);
        }
        [Fact]
        public void If_a_value_over_fifty_nine_is_given_throws_an_exception()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Minute(60));
            Assert.Contains("to 59", ex.Message);
        }
        [Fact]
        public void The_value_property_holds_the_minute()
        {
            Assert.Equal(new Minute(18).Value, 18);
        }
        [Fact]
        public void Equals_returns_false_if_argument_is_null()
        {
            Assert.False(new Minute(18).Equals(null));
        }
        [Fact]
        public void Equals_returns_false_if_argument_is_not_Minute_type()
        {
            Assert.False(new Minute(18).Equals(3));
        }
        [Fact]
        public void Equals_returns_false_if_argument_has_not_the_same_minute_value()
        {
            Assert.False(new Minute(18).Equals(new Minute(3)));
        }
        [Fact]
        public void Equals_returns_true_if_argument_has_the_same_minute_value()
        {
            Assert.True(new Minute(18).Equals(new Minute(18)));
        }
        [Fact]
        public void Equality_operator_act_as_Equals()
        {
            Assert.True(new Minute(18) == new Minute(18));
            Assert.False(new Minute(18) == new Minute(17));
        }
        [Fact]
        public void Distinct_operator_is_the_oposite_of_equality_operator()
        {
            Assert.False(new Minute(18) != new Minute(18));
            Assert.True(new Minute(18) != new Minute(17));
        }
    }
}
