namespace Altea.Classes.Stations
{
    using System;

    [Flags]
    public enum StationType
    {
        Normal = 0x01,
        Computer = 0x02,
        Audio = 0x03
    }
}
