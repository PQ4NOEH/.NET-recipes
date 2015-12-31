namespace Altea.Classes.WiseTank
{
    using Altea.Extensions;

    using System;

    using Newtonsoft.Json;

    public class TankStreamArticle
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long Id { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }

        [JsonProperty(PropertyName = "user", Required = Required.Always)]
        public string User { get; set; }

        [JsonProperty(PropertyName = "userFullName", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string UserFullName { get; set; }

        [JsonIgnore]
        public Guid AuthorId { get; set; }

        [JsonProperty(PropertyName = "author", Required = Required.Always)]
        public string Author { get; set; }

        [JsonProperty(PropertyName = "authorFullName", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string AuthorFullName { get; set; }

        [JsonProperty(PropertyName = "name", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "hasImage", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool HasImage { get; set; }

        [JsonProperty(PropertyName = "source", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Source { get; set; }

        [JsonIgnore]
        public byte[] FaviconArray { get; set; }

        [JsonProperty(PropertyName = "favicon", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Favicon
        {
            get
            {
                return this.FaviconArray == null ? null : Convert.ToBase64String(this.FaviconArray);
            }
        }

        [JsonProperty(PropertyName = "category", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? Category { get; set; }

        [JsonProperty(PropertyName = "isAppCategory", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? IsAppCategory { get; set; }

        [JsonProperty(PropertyName = "lead", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Lead { get; set; }

        [JsonProperty(PropertyName = "origin", Required = Required.Always)]
        public TankOrigin Origin { get; set; }

        [JsonProperty(PropertyName = "reference", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Reference { get; set; }

        [JsonProperty(PropertyName = "addDate", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public DateTime AddDate { get; set; }

        [JsonProperty(PropertyName = "upVotes", Required = Required.Always)]
        public int UpVotes { get; set; }

        [JsonProperty(PropertyName = "downVotes", Required = Required.Always)]
        public int DownVotes { get; set; }

        [JsonProperty(PropertyName = "karma", Required = Required.Always)]
        public decimal Karma { get; set; }

        [JsonProperty(PropertyName = "approved", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? Approved { get; set; }

        [JsonProperty(PropertyName = "deleted", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? Deleted { get; set; }

        [JsonProperty(PropertyName = "thinktankLevel", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public int? ThinktankLevel { get; set; }

        [JsonProperty(PropertyName = "userUpvoted", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? UserUpvoted { get; set; }

        [JsonProperty(PropertyName = "userKarmaed", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public decimal? UserKarmaed { get; set; }

        [JsonProperty(PropertyName = "canVote", Required = Required.Always)]
        public bool CanVote { get; set; }

        [JsonIgnore]
        public Guid? AssignedById { get; set; }

        [JsonProperty(PropertyName = "assignedBy", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string AssignedBy { get; set; }

        [JsonProperty(PropertyName = "assignedFullName", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string AssignedFullName { get; set; }

        [JsonProperty(PropertyName = "assignMessage", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string AssignMessage { get; set; }

        [JsonProperty(PropertyName = "articleNumber", Required = Required.Always)]
        public long ArticleNumber { get; set; }
    }
}
