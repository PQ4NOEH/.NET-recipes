using Davalor.MomProxy.Domain.Configuration;
using System;
using System.Linq;
using Xunit;

namespace Davalor.MomProxy.Domain.UnitTests.Configuration
{
    public class TimeRangeSpec
    {
        [Fact]
        public void InRange_If_a_date_is_between_timeRange_returns_true()
        {
            var sut = new TimeRange
            {
                From = new Time
                {
                    Hour = new Hour(20),
                    Minute = new Minute(5)
                },
                To = new Time
                {
                    Hour = new Hour(20),
                    Minute = new Minute(6)
                }
            };

            Assert.True(sut.InRange(DateTime.Parse("2000-02-02 20:05:00")));
            Assert.True(sut.InRange(DateTime.Parse("2000-02-02 20:06:00")));
        }
        [Fact]
        public void InRange_If_a_date_is_not_between_timeRange_returns_false()
        {
            var sut = new TimeRange
            {
                From = new Time
                {
                    Hour = new Hour(20),
                    Minute = new Minute(5)
                },
                To = new Time
                {
                    Hour = new Hour(20),
                    Minute = new Minute(6)
                }
            };

            Assert.False(sut.InRange(DateTime.Parse("2000-02-02 20:04:00")));
            Assert.False(sut.InRange(DateTime.Parse("2000-02-02 20:07:00")));
        }
        [Fact]
        public void If_From_property_is_null_it_is_invalid()
        {
            var sut = new TimeRange
            {
                To = new Time
                {
                    Hour = new Hour(20),
                    Minute = new Minute(6)
                }
            };
            Assert.False(sut.IsValid());
            Assert.Equal(sut.BrokenRules.Count(), 1);
            Assert.Contains("both From and To has to be configured", sut.BrokenRules.ElementAt(0));
        }
        [Fact]
        public void If_To_property_is_null_it_is_invalid()
        {
            var sut = new TimeRange
            {
                From = new Time
                {
                    Hour = new Hour(20),
                    Minute = new Minute(6)
                }
            };
            Assert.False(sut.IsValid());
            Assert.Equal(sut.BrokenRules.Count(), 1);
            Assert.Contains("both From and To has to be configured", sut.BrokenRules.ElementAt(0));
        }
        [Fact]
        public void If_From_is_equal_than_To_it_is_invalid()
        {
            var sut = new TimeRange
            {
                To = new Time
                {
                    Hour = new Hour(20),
                    Minute = new Minute(6)
                },
                From = new Time
                {
                    Hour = new Hour(20),
                    Minute = new Minute(6)
                }
            };
            Assert.False(sut.IsValid());
            Assert.Equal(sut.BrokenRules.Count(), 1);
            Assert.Contains("From has to be lower than To", sut.BrokenRules.ElementAt(0));
        }
        [Fact]
        public void If_To_is_lower_than_From_it_is_invalid()
        {
            var sut = new TimeRange
            {
                To = new Time
                {
                    Hour = new Hour(20),
                    Minute = new Minute(5)
                },
                From = new Time
                {
                    Hour = new Hour(20),
                    Minute = new Minute(6)
                }
            };
            Assert.False(sut.IsValid());
            Assert.Equal(sut.BrokenRules.Count(), 1);
            Assert.Contains("From has to be lower than To", sut.BrokenRules.ElementAt(0));
        }
    }
}
