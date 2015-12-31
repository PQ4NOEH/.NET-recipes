using System;
using System.Diagnostics.CodeAnalysis;

namespace AlteaLabs.Common.Contracts
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class LanguagePrefixesAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        public int DatabaseId { get; set; }

        /// <summary>
        /// Gets or sets the database prefix name.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the Javascript prefix name.
        /// </summary>
        [SuppressMessage(
            "StyleCop.CSharp.DocumentationRules",
            "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        public string Javascript { get; set; }

        /// <summary>
        /// Gets or sets the Microsoft Speak prefix name.
        /// </summary>
        public string MicrosoftSpeak { get; set; }

        /// <summary>
        /// Gets or sets the Abbyy prefix name.
        /// </summary>
        public string OcrAbbyy { get; set; }

        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        public string LongName { get; set; }

        /// <summary>
        /// Gets or sets the culture name
        /// </summary>
        public string Culture { get; set; }
    }
}
