using Davalor.SAP.Messages.Service;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace Davalor.SynchronizationManager.Repository.Service
{
    public class ServiceRepository: GenericDataService<ServiceAggregate>
    {
        public ServiceRepository(DbContext context) : base(context) { }
        public override async Task Delete(Guid aggregateId)
        {
            var match = await _dbSet.FindAsync(aggregateId);
            
            if (match != null)
            {
                var objToDelete = new List<object>();
                
                match.ServiceLevel.ForEach(s =>
                {
                    s.ServicePrice.ForEach(objToDelete.Add);
                    objToDelete.Add(s);
                });
                objToDelete.Add(match);
                objToDelete.Add(match.ServiceType);
                objToDelete.ForEach(((IObjectContextAdapter)_dbContext).ObjectContext.DeleteObject);
                await _dbContext.SaveChangesAsync();
            }
        }

        public override async Task Update(ServiceAggregate aggregate)
        {
            _dbSet.Attach(aggregate);
            _dbContext.Entry(aggregate).State = EntityState.Modified;
            aggregate.ServiceLevel.ForEach(l =>
                {
                    _dbContext.Entry(l).State = EntityState.Modified;
                    l.ServicePrice.ForEach(p => _dbContext.Entry(p).State = EntityState.Modified);
                });
            _dbContext.Entry(aggregate.ServiceType).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }

    
}
