// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Language.cs" company="ALTEA">
//   No copyright yet.
// </copyright>
// <summary>
//   Languages recognized and accepted by all applications.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Altea.Common.Classes
{
    /// <summary>
    /// Languages recognized and accepted by all applications.
    /// </summary>
    public enum Language
    {
        /// <summary>
        /// Default enumeration value, no language set.
        /// </summary>
        NoLanguage = 0,

        /// <summary>
        /// English language.
        /// </summary>
        [LanguagePrefixes(
            DatabaseId = 1,
            Database = "En",
            Javascript = "en",
            MicrosoftSpeak = "en",
            Culture = "en",
            OcrAbbyy = "English",
            ShortName = "En",
            LongName = "English")]
        English,

        /// <summary>
        /// French language.
        /// </summary>
        [LanguagePrefixes(
            DatabaseId = 2,
            Database = "Fr",
            Javascript = "fr",
            MicrosoftSpeak = "fr",
            Culture = "fr",
            OcrAbbyy = "French",
            ShortName = "Fr",
            LongName = "French")]
        French,

        /// <summary>
        /// German language.
        /// </summary>
        [LanguagePrefixes(DatabaseId = 3,
            Database = "De",
            Javascript = "de",
            MicrosoftSpeak = "de",
            Culture = "de",
            OcrAbbyy = "German,GermanLuxembourg,GermanNewSpelling",
            ShortName = "De",
            LongName = "German")]
        German,

        /// <summary>
        /// Portuguese language.
        /// </summary>
        [LanguagePrefixes(
            DatabaseId = 4,
            Database = "Pt",
            Javascript = "pt",
            MicrosoftSpeak = "pt",
            Culture = "pt",
            OcrAbbyy = "PortugueseStandard,PortugueseBrazilian",
            ShortName = "Pt",
            LongName = "Portuguese")]
        Portuguese,

        /// <summary>
        /// Spanish language.
        /// </summary>
        [LanguagePrefixes(
            DatabaseId = 5,
            Database = "Es",
            Javascript = "es",
            MicrosoftSpeak = "es",
            Culture = "es",
            OcrAbbyy = "Spanish,OldSpanish",
            ShortName = "Es",
            LongName = "Spanish")]
        Spanish,

        /// <summary>
        /// Catalan language.
        /// </summary>
        [LanguagePrefixes(
            DatabaseId = 6,
            Database = "Ca",
            Javascript = "ca",
            MicrosoftSpeak = "ca",
            Culture = "ca",
            OcrAbbyy = "Catalan",
            ShortName = "Ca",
            LongName = "Catalan")]
        Catalan,

        /// <summary>
        /// Basque language.
        /// </summary>
        [LanguagePrefixes(
            DatabaseId = 7,
            Database = "Ba",
            Javascript = "ba",
            MicrosoftSpeak = "ba",
            Culture = "eu",
            OcrAbbyy = "Basque",
            ShortName = "Ba",
            LongName = "Basque")]
        Basque,

        /// <summary>
        /// Galician language.
        /// </summary>
        [LanguagePrefixes(
            DatabaseId = 8,
            Database = "Ga",
            Javascript = "ga",
            MicrosoftSpeak = "ga",
            Culture = "gl",
            OcrAbbyy = "Galician",
            ShortName = "Ga",
            LongName = "Galician")]
        Galician
    }
}
