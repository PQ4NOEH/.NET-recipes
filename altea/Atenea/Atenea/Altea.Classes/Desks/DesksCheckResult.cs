namespace Altea.Classes.Desks
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Newtonsoft.Json;

    [DataContract]
    public class DesksCheckResult
    {
        [DataMember, JsonProperty(PropertyName = "checkStatus", Required = Required.Always)]
        public DesksCheckStatus CheckStatus { get; set; }

        [DataMember, JsonProperty(PropertyName = "code", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
        public string Code { get; set; }

        [DataMember, JsonProperty(PropertyName = "questions", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<int> Questions { get; set; }

        [DataMember, JsonProperty(PropertyName = "status", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<bool> Status { get; set; }

        [DataMember, JsonProperty(PropertyName = "answersStatus", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<IEnumerable<bool>> AnswersStatus { get; set; }

        [DataMember, JsonProperty(PropertyName = "answers", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<IEnumerable<string>> Answers { get; set; }
    }
}
