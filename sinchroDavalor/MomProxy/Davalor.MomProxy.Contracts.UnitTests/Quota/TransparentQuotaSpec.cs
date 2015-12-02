using Davalor.MomProxy.Domain.Quota;
using System;
using Xunit;

namespace Davalor.MomProxy.Domain.UnitTests.Quota
{
    public class TransparentQuotaSpec
    {
        [Fact]
        public void Fullfill_returns_false_if_the_number_of_elements_is_lower_than_the_configured()
        {
            int configuredQuota = new Random().Next(-10, 0);
            var sut = new TransparentQuota();
            for (int cont = configuredQuota; cont < 1; cont++)
            {
                Assert.False(sut.Fullfills(cont));
            }
        }
        [Fact]
        public void Fulfills_returns_true_if_number_of_elements_is_greaterThan_zero()
        {
            int configuredQuota = new Random().Next(3, 20);
            var sut = new TransparentQuota();
            for (int cont = 1; cont < configuredQuota; cont++)
            {
                Assert.True(sut.Fullfills(cont));
            }
        }
        
    }
}
