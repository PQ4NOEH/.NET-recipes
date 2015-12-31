namespace Altea.Classes.WiseNet
{
    using System;

    [Flags]
    public enum BlacklistStrength
    {
        None = 0x00,
        SourcesAndReferences = 0x01,
        TagEventAttributes = 0x02,
        ScriptAndNoscriptTags = 0x04
    }
}
