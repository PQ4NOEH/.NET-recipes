namespace Altea.Classes.WiseTank
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class TankArticleComment
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "user", Required = Required.Always)]
        public string User { get; set; }

        [JsonProperty(PropertyName = "comment", Required = Required.Always)]
        public string Comment { get; set; }

        [JsonProperty(PropertyName = "commentDate", Required = Required.Always)]
        public DateTime AddDate { get; set; }

        [JsonProperty(PropertyName = "upvotes", Required = Required.Always)]
        public int UpVotes { get; set; }

        [JsonProperty(PropertyName = "downvotes", Required = Required.Always)]
        public int DownVotes { get; set; }

        [JsonProperty(PropertyName = "approved", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? Approved { get; set; }

        [JsonProperty(PropertyName = "deleted", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool? Deleted { get; set; }

        [JsonProperty(PropertyName = "commentNumber", Required = Required.Always)]
        public int CommentNumber { get; set; }

        [JsonProperty(PropertyName = "children", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IEnumerable<TankArticleComment> Children { get; set; }
    }
}
