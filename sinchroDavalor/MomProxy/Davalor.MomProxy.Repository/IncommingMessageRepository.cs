using Davalor.Base.Library.Guards;
using Davalor.MomProxy.Domain;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Davalor.MomProxy.Repository
{
    public class IncommingMessageRepository : IIncommingMessageRepository
    {
        public IncommingMessageRepository()
        {
            if (!File.Exists("PendingMessages"))
            {
                using (File.Create("PendingMessages")) { }
            }
        }
        public void Save(NotNullOrWhiteSpaceString message)
        {
            File.AppendAllLines("PendingMessages", new List<string> { message });
        }

        public void Delete(NotNullOrWhiteSpaceString message)
        {
            var newContent = File.ReadAllLines("PendingMessages").ToList();
            newContent.Remove(message);
            File.WriteAllLines("PendingMessages",newContent);
        }

        public NotNullable<IEnumerable<NotNullOrWhiteSpaceString>> GetPending()
        {
            return File
                    .ReadAllLines("PendingMessages")
                    .Select(s => new NotNullOrWhiteSpaceString(s))
                    .ToList();
        }
    }
}
