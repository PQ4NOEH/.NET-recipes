namespace Altea.Classes.Desks
{
    public enum DesksBookPublicationType
    {
        [DesksBookPublicationTypeData(Position = 1)]
        Readings = 1,
        
        [DesksBookPublicationTypeData(Position = 2)]
        Exams = 2,

        [DesksBookPublicationTypeData(Position = 4)]
        Professional = 3,
        
        [DesksBookPublicationTypeData(Position = 3)]
        Academic = 4
    }
}