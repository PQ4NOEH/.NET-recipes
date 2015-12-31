namespace Altea.Classes.Desks
{
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    public class DesksBookList
    {
        [JsonProperty(PropertyName = "types", Required = Required.Always)]
        public IEnumerable<DesksBookType> Types { get; set; } 

        [JsonProperty(PropertyName = "articles", Required = Required.Always)]
        public IEnumerable<DesksBookPublication> Articles { get; set; }

        [JsonProperty(PropertyName = "books", Required = Required.Always)]
        public IEnumerable<DesksBookPublication> Books { get; set; }
        
        [JsonProperty(PropertyName = "collections", Required = Required.Always)]
        public IEnumerable<DesksBookCollection> Collections { get; set; }

        [JsonProperty(PropertyName = "assignments", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<DesksBookAssignment> Assignments { get; private set; }

        public void SetAssignments(IEnumerable<DesksBookAssignment> assignments)
        {
            this.Assignments = assignments;
            this.FilterPublications();
        }

        private void FilterPublications()
        {
            HashSet<long> assignments = new HashSet<long>(this.Assignments.Select(x => x.Id));

            foreach (DesksBookPublication book in this.Books)
            {
                book.Publications =
                    book.Publications.Where(article => assignments.Contains(article.Value.Id))
                        .ToDictionary(x => x.Key, x => x.Value);
            }

            foreach (DesksBookCollection collection in this.Collections)
            {
                collection.Publications =
                    collection.Publications.Where(x => assignments.Contains(x.Value.Id))
                        .ToDictionary(x => x.Key, x => x.Value);
            }

            this.Articles = this.Articles.Where(x => assignments.Contains(x.Id)).ToArray();
            this.Books = this.Books.Where(x => x.Publications.Count() != 0).ToArray();
            this.Collections = this.Collections.Where(x => x.Publications.Count() != 0).ToArray();
        }
    }
}
