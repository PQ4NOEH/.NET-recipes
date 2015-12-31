namespace Altea.Classes.Stax
{
    using System.Collections.Generic;

    public class StackExerciseAnswer
    {
        public long Id { get; set; }
        public IEnumerable<string> Answers { get; set; }
    }
}
