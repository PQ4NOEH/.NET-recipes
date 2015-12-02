using Davalor.SynchronizationManager.Repository.AuditLogon;
using Davalor.VisionLocal.Messages.AuditLogon;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System;
using Xunit;
using Moq;

namespace Davalor.SynchronizationManager.Repository.UnitTests.AuditLogon
{
    public class AnySpec
    {
        //[Fact]
        //public void CreateAuditLogon_saves_a_AuditLogon_via_context()
        //{
        //    var mockSet = new Mock<DbSet<AuditLogonAggregate>>();

        //    var mockContext = new Mock<AuditLogonDbContext>();
        //    mockContext.Setup(m => m.AuditLogon).Returns(mockSet.Object);
        //    mockContext.Setup(m => m.Set<AuditLogonAggregate>()).Returns(mockSet.Object);

        //    var service = new AuditLogonRepository(mockContext.Object);
        //    service.Save(new AuditLogonAggregate
        //        {
        //            Access = "Any Access",
        //            Id = Guid.NewGuid(),
        //            Ip = "127.0.0.1",
        //            AccessDate = DateTimeOffset.Now,
        //            TimeStamp = DateTimeOffset.Now,
        //            PartnerId = Guid.NewGuid(),
        //            UserName = "Any user Name"
        //        });

        //    mockSet.Verify(m => m.Add(It.IsAny<AuditLogonAggregate>()), Times.Once());
        //    mockContext.Verify(m => m.SaveChanges(), Times.Once()); 
        //}

    }
}
