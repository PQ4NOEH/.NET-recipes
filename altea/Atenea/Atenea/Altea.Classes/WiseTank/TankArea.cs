namespace Altea.Classes.WiseTank
{
    public enum TankArea
    {
        [TankAreaProperties(Position = 1, PlannerVisible = true)]
        Academic = 1,

        [TankAreaProperties(Position = 2, PlannerVisible = true)]
        Professional = 2,

        [TankAreaProperties(Position = 3, PlannerVisible = true)]
        Personal = 3
    }
}
