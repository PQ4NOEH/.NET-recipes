namespace Altea.Classes.Desks
{
    using System;

    [Flags]
    public enum DesksAssignmentStatus
    {
        None = 0x0000,
        Assigned = 0x0001,
        Started = 0x0002,
        Blocked = 0x0004,
        Finished = 0x0008,
        Certified = 0x0010,
        AllowPractice = 0x0080,
        RemoteAssigned = 0x0100,
        RemoteStarted = 0x0200,
        RemoteFinished = 0x0400
    }
}
