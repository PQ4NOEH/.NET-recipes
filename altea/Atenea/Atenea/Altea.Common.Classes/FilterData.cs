namespace Altea.Common.Classes
{
    using System;

    public class FilterData
    {
        public int Language { get; private set; }
        public string Word { get; private set; }
        public long Frequency { get; private set; }

        public FilterData(int language, string word, long frequency)
        {
            if (word == null)
            {
                throw new ArgumentNullException("word");
            }

            Language = language;
            Word = word.Trim().ToUpperInvariant();
            Frequency = frequency;
        }
    }
}
