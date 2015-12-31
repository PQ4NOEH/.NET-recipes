namespace Altea.Classes.WiseTank
{
    using System;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class TankAreaPropertiesAttribute : Attribute
    {
        public int Position { get; set; }
        public bool PlannerVisible { get; set; }

    }
}
