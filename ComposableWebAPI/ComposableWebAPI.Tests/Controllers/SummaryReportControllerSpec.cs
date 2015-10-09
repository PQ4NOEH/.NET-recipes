using ComposableWebAPI.Controllers;
using ComposableWebAPI.Models;
using NSubstitute;
using Report.Domain;
using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ComposableWebAPI.Tests.Controllers
{
    public class SummaryReportControllerSpec
    {
        [Fact]
        public async Task Get_returns_the_report()
        {
            var summaryReport = Substitute.For<ISummaryReport>();
            var sut = new SummaryReportController(summaryReport);

            var reportIdentity = new ReportIdentity
            {
                SessionId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                CountryId = Guid.NewGuid()
            };
            var expected = Encoding.UTF8.GetBytes(reportIdentity.ToString());
            summaryReport.Get(Arg.Any<ReportIdentity>()).Returns(expected);
            var actual = await sut.Get(reportIdentity.SessionId, reportIdentity.CountryId, reportIdentity.PatientId);
            Assert.Equal(expected, actual);
        }
    }
}
