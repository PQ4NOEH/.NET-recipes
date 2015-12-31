namespace Altea.Classes.WiseLab
{
    public enum WiseLabOrigin
    {
        [WiseLabProperty(
            MaxStatus = WiseLabStatus.None,
            HasPod = false,
            CanForcePod = false,
            HasPlanner = false,
            MinForcedScoutedWords = 0,
            MinForcedKeywords = 0,
            MinKeywords = 0,
            MaxKeywords = 0,
            MinForcedExpressions = 0,
            MinExpressions = 0,
            MaxExpressions = 0,
            MinLead = 0,
            MaxLead = 0)]
        Vocabulary = 1,

        [WiseLabProperty(
            MaxStatus = WiseLabStatus.Key,
            HasPod = false,
            CanForcePod = false,
            HasPlanner = false,
            MinForcedScoutedWords = 0,
            MinForcedKeywords = 0,
            MinKeywords = 3,
            MaxKeywords = 0,
            MinForcedExpressions = 0,
            MinExpressions = 0,
            MaxExpressions = 0,
            MinLead = 0,
            MaxLead = 0)]
        Grammar = 2,

        [WiseLabProperty(
            MaxStatus = WiseLabStatus.Key,
            HasPod = false,
            CanForcePod = false,
            HasPlanner = false,
            MinForcedScoutedWords = 3,
            MinForcedKeywords = 3,
            MinKeywords = 3,
            MaxKeywords = 3,
            MinForcedExpressions = 0,
            MinExpressions = 0,
            MaxExpressions = 0,
            MinLead = 0,
            MaxLead = 0)]
        TermDefis = 3,

        [WiseLabProperty(
            MaxStatus = WiseLabStatus.Wisdom,
            HasPod = true,
            CanForcePod = true,
            HasPlanner = true,
            MinForcedScoutedWords = 0,
            MinForcedKeywords = 3,
            MinKeywords = -1,
            MaxKeywords = -1,
            MinForcedExpressions = 3,
            MinExpressions = -1,
            MaxExpressions = -1,
            MinLead = -1,
            MaxLead = -1)]
        WiseNet = 4,

        [WiseLabProperty(
            MaxStatus = WiseLabStatus.Wisdom,
            HasPod = true,
            CanForcePod = true,
            HasPlanner = true,
            MinForcedScoutedWords = 0,
            MinForcedKeywords = 3,
            MinKeywords = -1,
            MaxKeywords = -1,
            MinForcedExpressions = 3,
            MinExpressions = -1,
            MaxExpressions = -1,
            MinLead = -1,
            MaxLead = -1)]
        WiseReader = 5,

        [WiseLabProperty(
            MaxStatus = WiseLabStatus.Scout,
            HasPod = false,
            CanForcePod = false,
            HasPlanner = false,
            MinForcedScoutedWords = 0,
            MinForcedKeywords = 0,
            MinKeywords = 0,
            MaxKeywords = 0,
            MinForcedExpressions = 0,
            MinExpressions = 0,
            MaxExpressions = 0,
            MinLead = 0,
            MaxLead = 0)]
        DesksIndex = 6,

        [WiseLabProperty(
            MaxStatus = WiseLabStatus.Scout,
            HasPod = false,
            CanForcePod = false,
            HasPlanner = false,
            MinForcedScoutedWords = 0,
            MinForcedKeywords = 0,
            MinKeywords = 0,
            MaxKeywords = 0,
            MinForcedExpressions = 0,
            MinExpressions = 0,
            MaxExpressions = 0,
            MinLead = 0,
            MaxLead = 0)]
        DesksGrammar = 7,

        [WiseLabProperty(
            MaxStatus = WiseLabStatus.Scout,
            HasPod = false,
            CanForcePod = false,
            HasPlanner = false,
            MinForcedScoutedWords = 0,
            MinForcedKeywords = 0,
            MinKeywords = 0,
            MaxKeywords = 0,
            MinForcedExpressions = 0,
            MinExpressions = 0,
            MaxExpressions = 0,
            MinLead = 0,
            MaxLead = 0)]
        DesksExams = 8,

        [WiseLabProperty(
            MaxStatus = WiseLabStatus.Scout,
            HasPod = false,
            CanForcePod = false,
            HasPlanner = false,
            MinForcedScoutedWords = 0,
            MinForcedKeywords = 0,
            MinKeywords = 0,
            MaxKeywords = 0,
            MinForcedExpressions = 0,
            MinExpressions = 0,
            MaxExpressions = 0,
            MinLead = 0,
            MaxLead = 0)]
        DesksExamsTests = 9,

        [WiseLabProperty(
            MaxStatus = WiseLabStatus.Scout,
            HasPod = false,
            CanForcePod = false,
            HasPlanner = false,
            MinForcedScoutedWords = 0,
            MinForcedKeywords = 0,
            MinKeywords = 0,
            MaxKeywords = 0,
            MinForcedExpressions = 0,
            MinExpressions = 0,
            MaxExpressions = 0,
            MinLead = 0,
            MaxLead = 0)]
        DesksInfo = 10,

        [WiseLabProperty(
            MaxStatus = WiseLabStatus.Scout,
            HasPod = false,
            CanForcePod = false,
            HasPlanner = false,
            MinForcedScoutedWords = 0,
            MinForcedKeywords = 0,
            MinKeywords = 0,
            MaxKeywords = 0,
            MinForcedExpressions = 0,
            MinExpressions = 0,
            MaxExpressions = 0,
            MinLead = 0,
            MaxLead = 0)]
        Extra = 11,

        [WiseLabProperty
            (MaxStatus = WiseLabStatus.Wisdom,
            HasPod = true,
            CanForcePod = true,
            HasPlanner = false,
            MinForcedScoutedWords = 0,
            MinForcedKeywords = 3,
            MinKeywords = 3,
            MaxKeywords = 3,
            MinForcedExpressions = 0,
            MinExpressions = 0,
            MaxExpressions = 0,
            MinLead = 0,
            MaxLead = 0)]
        ExtraFull = 12
    }
}
