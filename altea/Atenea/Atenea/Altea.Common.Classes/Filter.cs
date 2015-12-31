namespace Altea.Common.Classes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Filter
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int[] Languages { get; private set; }

        private IEnumerable<FilterData> _data;

        public IEnumerable<FilterData> Data
        {
            get
            {
                return _data;
            }
            set
            {
                if (_data == null)
                {
                    _data = value;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public bool DataLoaded
        {
            get
            {
                return _data != null;
            }
        }

        public Filter(int id, string name, IEnumerable<int> languages)
        {
            Id = id;
            Name = name;
            Languages = languages as int[] ?? languages.ToArray();

            Data = null;
        }

        private static readonly Filter EmptyFilter =
            new Filter(0, "No filter", Enumerable.Empty<int>())
                {
                    Data = Enumerable.Empty<FilterData>()
                };

        public static Filter Empty
        {
            get
            {
                return EmptyFilter;
            }
        }
    }
}
