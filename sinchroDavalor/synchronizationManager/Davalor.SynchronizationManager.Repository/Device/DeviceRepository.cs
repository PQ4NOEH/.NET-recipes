using Davalor.SAP.Messages.Device;
using System.Data.Entity;
using System.Threading.Tasks;
using System;

namespace Davalor.SynchronizationManager.Repository.Device
{
    public class DeviceRepository: GenericDataService<DeviceAggregate>
    {
        public DeviceRepository(DbContext context) : base(context) { }

        public override async Task Update(DeviceAggregate aggregate)
        {
            _dbSet.Attach(aggregate);
            _dbContext.Entry(aggregate).State = EntityState.Modified;
            _dbContext.Entry(aggregate.DeviceGroup).State = EntityState.Modified;
            _dbContext.Entry(aggregate.DeviceGroup.DeviceType).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
