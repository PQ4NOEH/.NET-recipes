namespace Altea.Models.Stax
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Altea.Classes.Stax;
    using Altea.Common.Classes;

    [DataContract]
    public class StackSaveExerciseDataModel
    {
        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public Guid AppId { get; set; }

        [DataMember]
        public bool Remote { get; set; }

        [DataMember]
        public Language From { get; set; }

        [DataMember]
        public Language To { get; set; }

        [DataMember]
        public StackType Type { get; set; }

        [DataMember]
        public int StackNum { get; set; }

        [DataMember]
        public IEnumerable<StackExerciseAnswer> Exercises { get; set; }

        [DataMember]
        public bool Status { get; set; }

        [DataMember]
        public TimeSpan Time { get; set; }

        [DataMember]
        public int RetryCooldown { get; set; }

        [DataMember]
        public int InboxErrors { get; set; }

        [DataMember]
        public int OffsetDate { get; set; }
    }
}
