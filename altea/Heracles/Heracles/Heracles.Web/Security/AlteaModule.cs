namespace Heracles.Web.Security
{
    public class AlteaModule
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public int Priority { get; set; }
        public int Position { get; set; }
        public AlteaModuleRoute Route { get; set; }
    }
}