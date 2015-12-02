using Davalor.MomProxy.Domain.Quota;
using System;
using Xunit;

namespace Davalor.MomProxy.Domain.UnitTests.Quota
{
    public class NumberOfElementsQuotaSpec
    {
        [Fact]
        public void Fullfill_returns_false_if_the_number_of_elements_is_lower_than_the_configured()
        {
            uint configuredQuota = (uint)new Random().Next(3,20); 
            var sut = new NumberOfElementsQuota(configuredQuota);
            for(int cont = -1;cont< configuredQuota; cont++ )
            {
                Assert.False(sut.Fullfills(cont));
            }
        }
        [Fact]
        public void Fullfill_returns_true_if_the_number_of_elements_is_equal_to_the_configured()
        {
            uint configuredQuota = (uint)new Random().Next(3, 20);
            var sut = new NumberOfElementsQuota(configuredQuota);
            Assert.True(sut.Fullfills((int)configuredQuota));
        }
        [Fact]
        public void Fullfill_returns_true_if_the_number_of_elements_is_greaterThan_the_configured()
        {
            uint configuredQuota = (uint)new Random().Next(3, 20);
            var sut = new NumberOfElementsQuota(configuredQuota);
            Assert.True(sut.Fullfills((int)configuredQuota + 1));
        }
    }
}
