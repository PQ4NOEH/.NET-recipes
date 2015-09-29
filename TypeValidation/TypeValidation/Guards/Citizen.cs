using System;

namespace TypeValidation.Guards
{
    public class Citizen
    {
        private int _age;
        public int Age
        {
            get  {return _age;}
            set
            {
                Ensure.IsInRange<int>(value, 16, int.MaxValue, "Age must be sixteen or greater.");
                _age = value;
            }
        }

        
        public DateTime BirtDate
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    }
}
