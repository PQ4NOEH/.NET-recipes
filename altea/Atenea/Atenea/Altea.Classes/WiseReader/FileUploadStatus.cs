namespace Altea.Classes.WiseReader
{
    using System;

    [Flags]
    public enum FileUploadStatus
    {
        Valid = 0x000,
        Exists = 0x002,
        Processed = 0x001,
        Invalid = 0x007,

        UnknownError = 0x008,
        MimeTypeError = 0x018,
        FileTooBigError = 0x028,
        QuotaExceededError = 0x048,
        CantProcessError = 0x088,

        Converted = 0x101
    }
}
