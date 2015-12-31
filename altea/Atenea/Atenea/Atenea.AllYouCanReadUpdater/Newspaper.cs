namespace Atenea.AllYouCanReadUpdater
{
    using System;

    internal class Newspaper
    {
        public string Name { get; set; }
        public Uri Url { get; set; }
        public Uri Favicon { get; set; }
        public Uri Image { get; set; }
        
        public override bool Equals(object obj)
        {
            Newspaper newspaper = obj as Newspaper;
            if (object.ReferenceEquals(newspaper, null))
            {
                return false;
            }

            if (object.ReferenceEquals(this, newspaper))
            {
                return true;
            }

            return this.Name.Equals(newspaper.Name, StringComparison.InvariantCultureIgnoreCase)
                   && this.Url.Equals(newspaper.Url);
        }

        public bool Equals(Newspaper newspaper)
        {
            if (object.ReferenceEquals(newspaper, null))
            {
                return false;
            }

            if (object.ReferenceEquals(this, newspaper))
            {
                return true;
            }

            return this.Name.Equals(newspaper.Name, StringComparison.InvariantCultureIgnoreCase)
                   && this.Url.Equals(newspaper.Url);
        }

        public override int GetHashCode()
        {
            return (this.Name.ToLowerInvariant().GetHashCode() * 27) + (this.Url.GetHashCode() * 17);
        }

        public static bool operator ==(Newspaper item1, Newspaper item2)
        {
            if (object.ReferenceEquals(item1, item2))
            {
                return true;
            }

            if (object.ReferenceEquals(item1, null) || object.ReferenceEquals(item2, null))
            {
                return false;
            }

            return item1.Name.Equals(item2.Name, StringComparison.InvariantCultureIgnoreCase)
                   && item1.Url.Equals(item2.Url);
        }

        public static bool operator !=(Newspaper item1, Newspaper item2)
        {
            return !(item1 == item2);
        }
    }
}
