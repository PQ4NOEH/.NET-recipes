
using Davalor.SynchronizationManager.Domain.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Davalor.SAP.Messages.Location
{
    public partial class LocationAggregate : ISynchroAggregateRoot
    {
        public LocationAggregate()
        {
        }

        public Guid Id { get; set; }

        [StringLength(40)]
        public string City { get; set; }

        [StringLength(100)]
        public string Street { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        public Guid? CountryId { get; set; }

        public Guid? RegionId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
