namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class DesksBookPublication : IDesksBookData
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "publicationFormat", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public DesksBookPublicationFormat PublicationFormat { get; set; }

        [JsonProperty(PropertyName = "publicationType", Required = Required.Always)]
        public DesksBookPublicationType PublicationType { get; set; }

        [JsonProperty(PropertyName = "exerciseTypes", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<int> ExerciseTypes { get; set; }

        [JsonProperty(PropertyName = "levels", Required = Required.Always)]
        public IEnumerable<int> Levels { get; set; }

        [JsonProperty(PropertyName = "authors", Required = Required.Always)]
        public IEnumerable<string> Authors { get; set; }

        [JsonProperty(PropertyName = "publishers", Required = Required.Always)]
        public IEnumerable<string> Publishers { get; set; }

        [JsonProperty(PropertyName = "categories", Required = Required.Always)]
        public IEnumerable<int> Categories { get; set; }

        [JsonProperty(PropertyName = "tags", Required = Required.Always)]
        public IEnumerable<int> Tags { get; set; }
        
        [JsonProperty(PropertyName = "title", Required = Required.Always)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "subtitle", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public string Subtitle { get; set; }

        [JsonProperty(PropertyName = "isTextArticle", Required = Required.Always)]
        public bool IsTextArticle { get; set; }

        [JsonProperty(PropertyName = "isCollectionArticle", Required = Required.Always)]
        public bool IsCollectionArticle { get; set; }

        [JsonProperty(PropertyName = "publications", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public IDictionary<int, DesksBookPublication> Publications { get; set; } 
    }
}
