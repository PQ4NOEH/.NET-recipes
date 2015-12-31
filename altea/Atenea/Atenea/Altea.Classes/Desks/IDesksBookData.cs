namespace Altea.Classes.Desks
{
    using System.Collections.Generic;

    public interface IDesksBookData
    {
        long Id { get; set; }
        
        IEnumerable<int> ExerciseTypes { get; set; }

        IEnumerable<int> Categories { get; set; }

        IEnumerable<int> Tags { get; set; }

        IDictionary<int, DesksBookPublication> Publications { get; set; } 
    }
}