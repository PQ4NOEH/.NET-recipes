// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguagePrefixType.cs" company="ALTEA">
//   No copyright yet.
// </copyright>
// <summary>
//   Code sections types with language prefixes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Altea.Common.Classes
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Code sections with language prefixes.
    /// </summary>
    public enum LanguagePrefixType
    {
        /// <summary>
        /// SQL and Graph databases id.
        /// </summary>
        DatabaseId,

        /// <summary>
        /// SQL and Graph databases name.
        /// </summary>
        Database,

        /// <summary>
        /// Javascript code.
        /// </summary>
        [SuppressMessage(
            "StyleCop.CSharp.DocumentationRules",
            "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        Javascript,

        /// <summary>
        /// Microsoft Speak API.
        /// </summary>
        MicrosoftSpeak,

        /// <summary>
        /// Abbyy Cloud OCR SDK.
        /// </summary>
        OcrAbbyy,

        /// <summary>
        /// Short name.
        /// </summary>
        ShortName,

        /// <summary>
        /// Full name.
        /// </summary>
        LongName
    }
}
