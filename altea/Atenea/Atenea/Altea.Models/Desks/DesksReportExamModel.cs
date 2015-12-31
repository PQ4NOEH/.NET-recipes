namespace Altea.Models.Desks
{
    using System;
    using System.Runtime.Serialization;

    using Altea.Classes.Desks;

    [DataContract]
    public class DesksReportExamModel
    {
        [DataMember]
        public Guid User { get; set; }

        [DataMember]
        public DesksReportType Type { get; set; }

        [DataMember]
        public int? Part { get; set; }

        [DataMember]
        public int? Question { get; set; }

        [DataMember]
        public DesksReportFeedbackType FeedbackType { get; set; }

        [DataMember]
        public string Feedback { get; set; }

        [DataMember]
        public bool Block { get; set; }
    }
}
