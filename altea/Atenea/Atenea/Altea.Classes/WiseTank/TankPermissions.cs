namespace Altea.Classes.WiseTank
{
    using Newtonsoft.Json;

    public class TankPermissions
    {
        [JsonProperty(PropertyName = "listUsers", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? ListUsers { get; set; }

        [JsonProperty(PropertyName = "inviteUsers", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? InviteUsers { get; set; }

        [JsonProperty(PropertyName = "readArticles", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? ReadArticles { get; set; }

        [JsonProperty(PropertyName = "writeArticles", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? WriteArticles { get; set; }

        [JsonProperty(PropertyName = "unmoderatedArticles", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? UnmoderatedArticles { get; set; }

        [JsonProperty(PropertyName = "voteArticles", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? VoteArticles { get; set; }

        [JsonProperty(PropertyName = "assignArticles", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? AssignArticles { get; set; }

        [JsonProperty(PropertyName = "approveArticles", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? ApproveArticles { get; set; }

        [JsonProperty(PropertyName = "deleteArticles", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? DeleteArticles { get; set; }

        [JsonProperty(PropertyName = "readComments", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? ReadComments { get; set; }

        [JsonProperty(PropertyName = "writeComments", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? WriteComments { get; set; }

        [JsonProperty(PropertyName = "writePrivateComments", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? WritePrivateComments { get; set; }

        [JsonProperty(PropertyName = "unmoderatedComments", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? UnmoderatedComments { get; set; }

        [JsonProperty(PropertyName = "approveComments", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? ApproveComments { get; set; }

        [JsonProperty(PropertyName = "deleteComments", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? DeleteComments { get; set; }

        [JsonProperty(PropertyName = "hideAuthor", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? HideAuthor { get; set; }

        [JsonProperty(PropertyName = "createSubtimelines", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? CreateSubTimelines { get; set; }

        [JsonProperty(PropertyName = "overrideOwnArticles", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public bool? OverrideOwnArticles { get; set; }
    }
}
