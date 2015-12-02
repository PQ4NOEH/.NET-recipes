
using System;

namespace Davalor.VisionLocal.Messages.Session
{
    public class Diagnosis
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Appraisal { get; set; }

        public Guid SessionId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public virtual SessionAggregate Session { get; set; }
    }
}
