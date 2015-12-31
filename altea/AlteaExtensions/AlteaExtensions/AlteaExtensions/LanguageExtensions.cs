namespace Altea.Extensions
{
    using System;
    using System.Collections.Concurrent;

    using Altea.Common.Classes;

    public static class LanguageExtensions
    {
        private static readonly Type LanguageT = typeof(Language);
        private static readonly Type LanguagePrefixT = typeof(LanguagePrefixType);

        private static readonly ConcurrentDictionary<Language, int> DatabaseIds =
            new ConcurrentDictionary<Language, int>();

        private static readonly ConcurrentDictionary<Tuple<Language, LanguagePrefixType>, string> Prefixes =
            new ConcurrentDictionary<Tuple<Language, LanguagePrefixType>, string>();

        public static string GetPrefix(this Language @this, LanguagePrefixType prefixType)
        {
            if (@this == Language.NoLanguage || !Enum.IsDefined(LanguageT, @this))
            {
                throw new ArgumentException("No valid language selected", "this");
            }

            if (prefixType == LanguagePrefixType.DatabaseId || !Enum.IsDefined(LanguagePrefixT, prefixType))
            {
                throw new ArgumentException("No valid prefix type selected", "this");
            }

            Tuple<Language, LanguagePrefixType> tuple = Tuple.Create(@this, prefixType);

            string prefix;
            if (!Prefixes.TryGetValue(tuple, out prefix))
            {
                LanguagePrefixesAttribute attribute = @this.GetAttribute<LanguagePrefixesAttribute>();

                switch (prefixType)
                {
                    case LanguagePrefixType.Database:
                        prefix = attribute.Database;
                        break;

                    case LanguagePrefixType.Javascript:
                        prefix = attribute.Javascript;
                        break;

                    case LanguagePrefixType.MicrosoftSpeak:
                        prefix = attribute.MicrosoftSpeak;
                        break;

                    case LanguagePrefixType.OcrAbbyy:
                        prefix = attribute.OcrAbbyy;
                        break;

                    case LanguagePrefixType.ShortName:
                        prefix = attribute.ShortName;
                        break;

                    case LanguagePrefixType.LongName:
                        prefix = attribute.LongName;
                        break;

                    default:
                        return null;
                }

                if (prefix != null)
                {
                    Prefixes.TryAdd(tuple, prefix);
                }
            }

            return prefix;
        }

        public static int GetDatabaseId(this Language @this)
        {
            
            if (@this == Language.NoLanguage || !Enum.IsDefined(LanguageT, @this))
            {
                throw new ArgumentException("No valid language selected", "this");
            }

            int id;
            if (!DatabaseIds.TryGetValue(@this, out id))
            {
                LanguagePrefixesAttribute attribute = @this.GetAttribute<LanguagePrefixesAttribute>();
                id = attribute.DatabaseId;

                if (id > 0)
                {
                    DatabaseIds.TryAdd(@this, id);
                }
            }

            return id;
        }

        public static Language ParseWordLanguagePrefix(this string @this, LanguagePrefixType prefixType)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("this");
            }

            if (prefixType == LanguagePrefixType.DatabaseId || !Enum.IsDefined(LanguagePrefixT, prefixType))
            {
                throw new ArgumentException("No valid prefix type selected", "this");
            }

            foreach (Language language in Enum.GetValues(LanguageT))
            {
                if (language == Language.NoLanguage)
                {
                    continue;
                }

                string prefix = language.GetPrefix(prefixType);
                if (prefix != null && prefix.Equals(@this))
                {
                    return language;
                }
            }

            return Language.NoLanguage;
        }

        public static Language ParseWordLanguageDatabaseId(this int @this)
        {
            if (@this <= 0)
            {
                throw new ArgumentOutOfRangeException("this");
            }

            foreach (Language language in Enum.GetValues(LanguageT))
            {
                if (language == Language.NoLanguage)
                {
                    continue;
                }

                int prefix = language.GetDatabaseId();
                if (prefix > 0 && prefix.Equals(@this))
                {
                    return language;
                }
            }

            return Language.NoLanguage;
        }
    }
}
