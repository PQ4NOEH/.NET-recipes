namespace Atenea.AllYouCanReadUpdater
{
    using System.Collections.Generic;

    internal class Carousel
    {
        private readonly string name;
        private readonly IEnumerable<Magazine> magazines;
        private readonly HashSet<Carousel> children;

        public int Id { get; set; }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public IEnumerable<Magazine> Magazines
        {
            get
            {
                return this.magazines;
            }
        }

        public IEnumerable<Carousel> Children
        {
            get
            {
                return this.children;
            }
        }

        public Carousel(string name, IEnumerable<Magazine> magazines)
        {
            this.name = name;
            this.magazines = magazines;

            this.children = new HashSet<Carousel>();
        }

        public void AddChild(Carousel carousel)
        {
            this.children.Add(carousel);
        }
    }
}
