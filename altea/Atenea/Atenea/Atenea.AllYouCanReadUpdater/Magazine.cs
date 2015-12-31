namespace Atenea.AllYouCanReadUpdater
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Magazine
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Uri Url { get; set; }
        public Uri Cover { get; set; }
        
        public override bool Equals(object obj)
        {
            Magazine magazine = obj as Magazine;
            if (object.ReferenceEquals(magazine, null))
            {
                return false;
            }

            if (object.ReferenceEquals(this, magazine))
            {
                return true;
            }

            return this.Name.Equals(magazine.Name, StringComparison.InvariantCultureIgnoreCase)
                   && this.Url.Equals(magazine.Url);
        }

        public bool Equals(Magazine magazine)
        {
            if (object.ReferenceEquals(magazine, null))
            {
                return false;
            }

            if (object.ReferenceEquals(this, magazine))
            {
                return true;
            }

            return this.Name.Equals(magazine.Name, StringComparison.InvariantCultureIgnoreCase)
                   && this.Url.Equals(magazine.Url);
        }

        public override int GetHashCode()
        {
            return (this.Name.ToLowerInvariant().GetHashCode() * 27) + (this.Url.GetHashCode() * 17);
        }

        public static bool operator ==(Magazine item1, Magazine item2)
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

        public static bool operator !=(Magazine item1, Magazine item2)
        {
            return !(item1 == item2);
        }
    }
}
