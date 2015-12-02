using Davalor.MomProxy.Domain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Davalor.MomProxy.Domain.UnitTests.Configuration
{
    public class TimeSpec
    {
        [Fact]
        public void Equals_returns_false_if_argument_is_null()
        {
            Assert.False(new Time().Equals(null));
        }
        [Fact]
        public void Equals_returns_true_if_argument_is_the_same_reference()
        {
            var time = new Time();
            Assert.True(time.Equals(time));
        }
        [Fact]
        public void Equals_returns_false_if_argument_is_of_different_type()
        {
            Assert.False(new Time().Equals(DateTime.Now));
        }
        [Fact]
        public void Equals_returns_false_if_hour_is_null_in_a_an_not_in_b()
        {
            var a = new Time
            {
                Minute = new Minute(2)
            };
            var b = new Time
            {
                Hour = new Hour(2),
                Minute = new Minute(2)
            };
            Assert.False(a.Equals(b));
        }
        [Fact]
        public void Equals_returns_false_if_minute_is_null_in_a_an_not_in_b()
        {
            var a = new Time
            {
                Hour = new Hour(2),
                Minute = new Minute(2)
            };
            var b = new Time
            {
                Hour = new Hour(2)
            };
            Assert.False(a.Equals(b));
        }
        [Fact]
        public void operator_equals_returns_true_if_both_elements_are_null()
        {
            Time nullTime = null;
            Assert.True(nullTime == null);
        }
        [Fact]
        public void operator_equals_returns_false_if_only_one_of_the_elements_are_null()
        {
            Time nullTime = null;
            Assert.False(nullTime == new Time());
            Assert.False(new Time() == nullTime);
        }
        [Fact]
        public void operator_Equals_if_first_object_is_not_null_works_as_equals()
        {
            var timeA = new Time();
            var timeB = timeA;
            Assert.True(timeA == timeB);
        }
        [Fact]
        public void operator_distinct_is_negated_operator_equals()
        {
            var timeA = new Time();
            var timeB = timeA;
            Assert.False(timeA != timeB);
        }
        [Fact]
        public void Operator_greaterThan_returns_false_if_any_of_the_elements_is_null()
        {
            Assert.False(new Time() > null);
            Assert.False(null > new Time());
            Assert.False(null > null);
        }
        [Fact]
        public void Operator_lessThan_returns_false_if_any_of_the_elements_is_null()
        {
            Time timeA = new Time();
            Time timeB = null;
            Assert.False(timeA < timeB);
            Assert.False(timeB < timeA);
            Assert.False(timeB < null);
        }
        [Fact]
        public void Operator_greaterThan_returns_true_if_hour_elementA_is_lower_than_elementBs()
        {
            Time timeA = new Time
                {
                    Hour = new Hour(7),
                    Minute = new Minute(4)
                };
            Time timeB = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(4)
            }; ;
            Assert.True(timeA > timeB);
        }
        [Fact]
        public void Operator_greaterThan_returns_true_if_both_elements_has_the_same_hour_and_elementA_minute_is_greater_than_bs()
        {
            Time timeA = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(5)
            };
            Time timeB = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(4)
            }; ;
            Assert.True(timeA > timeB);
        }
        [Fact]
        public void Operator_greaterThan_returns_false_if_a_b_are_equals()
        {
            Time timeA = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(5)
            };
            Time timeB = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(5)
            }; ;
            Assert.False(timeA > timeB);
        }
        [Fact]
        public void Operator_greaterThan_returns_false_if_b_hours_is_greater()
        {
            Time timeA = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(5)
            };
            Time timeB = new Time
            {
                Hour = new Hour(7),
                Minute = new Minute(5)
            }; 
            Assert.False(timeA > timeB);
        }
        [Fact]
        public void Operator_greaterThan_returns_false_if_hours_are_the_same_and_b_minutes_greaters()
        {
            Time timeA = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(5)
            };
            Time timeB = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(6)
            }; 
            Assert.False(timeA > timeB);
        }


        [Fact]
        public void Operator_lessThan_returns_false_if_hour_elementA_is_lower_than_elementBs()
        {
            Time timeA = new Time
            {
                Hour = new Hour(7),
                Minute = new Minute(4)
            };
            Time timeB = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(4)
            }; ;
            Assert.False(timeA < timeB);
        }
        [Fact]
        public void Operator_lessThan_returns_false_if_both_elements_has_the_same_hour_and_elementA_minute_is_greater_than_bs()
        {
            Time timeA = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(5)
            };
            Time timeB = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(4)
            }; ;
            Assert.False(timeA < timeB);
        }
        [Fact]
        public void Operator_lessThan_returns_true_if_a_b_are_equals()
        {
            Time timeA = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(5)
            };
            Time timeB = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(5)
            }; ;
            Assert.True(timeA < timeB);
        }
        [Fact]
        public void Operator_lessThan_returns_true_if_b_hours_is_greater()
        {
            Time timeA = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(5)
            };
            Time timeB = new Time
            {
                Hour = new Hour(7),
                Minute = new Minute(5)
            }; ;
            Assert.True(timeA < timeB);
        }
        [Fact]
        public void Operator_lessThan_returns_true_if_hours_are_the_same_and_b_minutes_greaters()
        {
            Time timeA = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(5)
            };
            Time timeB = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(6)
            }; ;
            Assert.True(timeA < timeB);
        }
        [Fact]
        public void Operator_greateThanOrEqual_is_a_combination_of_equal_operator_and_lessThan_operator()
        {
            Time timeA = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(5)
            };
            Time timeB = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(6)
            }; ;
            Assert.False(timeA >= timeB);
        }
        [Fact]
        public void Operator_lessThaOrEqual_is_a_combination_of_equal_operator_and_lessThan_operator()
        {
             Time timeA = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(5)
            };
            Time timeB = new Time
            {
                Hour = new Hour(6),
                Minute = new Minute(6)
            }; ;
            Assert.True(timeA <= timeB);
        }
        
    }
}
