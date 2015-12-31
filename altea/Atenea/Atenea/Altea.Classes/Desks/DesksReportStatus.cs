namespace Altea.Classes.Desks
{
    using System;

    [Flags]
    public enum DesksReportStatus
    {
        NotReported = 0x00,
        Reported = 0x01,
        Blocked = 0x02,
        UserReported = 0x08
    }
}