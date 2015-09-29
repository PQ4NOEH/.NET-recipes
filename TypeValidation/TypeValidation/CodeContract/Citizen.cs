using System;

namespace TypeValidation.CodeContract
{
   
    public class Citizen : ICitizen
    {

        public int Age
        {
            get;
            set;
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
