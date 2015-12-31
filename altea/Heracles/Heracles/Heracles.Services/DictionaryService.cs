namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;

    using Altea.Classes.WiseLab;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;
    using Altea.Models.Dictionary;

    using MicrosoftTranslator;

    public abstract class DictionaryService : Service<IDictionaryChannel>
    {
        public static TranslatedWord TranslateWord(string word, Language from, Language to)
        {
            string normalizedWord = word.Trim().ToLowerInvariant();
            TranslatedWord translatedWord = null;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[DICTIONARY_Translate]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@word",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    normalizedWord);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language_from",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    from.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language_to",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    to.GetDatabaseId());

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Dictionary,
                    reader => translatedWord = ReaderTranslateWords(reader).SingleOrDefault());
            }

            NormalizeTranslation(translatedWord);
            return translatedWord;
        }

        public static IEnumerable<TranslatedWord> TranslateWords(IEnumerable<string> words, Language from, Language to)
        {
            string[] normalizedWords = words.Select(x => x.Trim().ToLowerInvariant()).Distinct().ToArray();
            List<TranslatedWord> translatedWords = null;

            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.StoredProcedure, "[dbo].[DICTIONARY_TranslateList]"))
            using (DataTable wordsTable = new DataTable())
            {
                wordsTable.Columns.Add("word", typeof(string));

                foreach (string word in normalizedWords)
                {
                    DataRow row = wordsTable.NewRow();
                    row["word"] = word;
                    wordsTable.Rows.Add(row);
                }

                SqlDatabaseManager.AddParameter(
                    command,
                    "@words",
                    ParameterDirection.Input,
                    "[dbo].[wordlist]",
                    wordsTable);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language_from",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    from.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language_to",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    to.GetDatabaseId());

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Dictionary,
                    reader => translatedWords = ReaderTranslateWords(reader));
            }

            foreach (TranslatedWord translatedWord in translatedWords)
            {
                NormalizeTranslation(translatedWord);
            }

            return translatedWords;
        }

        private static List<TranslatedWord> ReaderTranslateWords(SqlDataReader reader)
        {
            Dictionary<string, TranslatedWord> translations = new Dictionary<string, TranslatedWord>();

            string lastWord = null;
            TranslatedWord lastTranslatedWord = null;
            List<Translation> lastTranslations = null;
            int lastType = 0;
            HashSet<string> lastTypeTranslations = null;
            HashSet<string> lastDefinedTranslations = null;

            while (reader.Read())
            {
                string word = (string)reader["word"];
                string translation = (string)reader["translation"];
                int type = (int)reader["type"];

                if (word != lastWord)
                {
                    lastWord = word;
                    lastType = 0;

                    if (translations.ContainsKey(lastWord))
                    {
                        lastTranslatedWord = translations[word];
                        lastTranslations = lastTranslatedWord.OtherTranslations as List<Translation>;
                    }
                    else
                    {
                        lastTranslations = new List<Translation>();
                        lastTranslatedWord = new TranslatedWord
                            {
                                Word = lastWord,
                                SuggestedTranslation = null,
                                OtherTranslations = lastTranslations
                            };

                        translations.Add(lastWord, lastTranslatedWord);

                        lastDefinedTranslations = new HashSet<string>();
                    }
                }

                if (type == 1)
                {
                    if (lastTranslatedWord.SuggestedTranslation == null)
                    {
                        lastTranslatedWord.SuggestedTranslation = translation;
                        lastDefinedTranslations.Add(translation.ToLowerInvariant());
                    }
                }
                else
                {
                    if (type != lastType)
                    {
                        lastType = type;
                        Translation lastTranslationType = lastTranslations.SingleOrDefault(x => x.Type == lastType);

                        if (lastTranslationType == null)
                        {
                            lastTypeTranslations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            lastTranslationType = new Translation
                                {
                                    Type = lastType,
                                    Translations = lastTypeTranslations
                                };
                            lastTranslations.Add(lastTranslationType);
                        }
                        else
                        {
                            lastTypeTranslations = lastTranslationType.Translations as HashSet<string>;
                        }
                    }

                    lastTypeTranslations.Add(translation);
                }
            }

            return translations.Values.ToList();
        }

        private static void NormalizeTranslation(TranslatedWord translatedWord)
        {
            if (translatedWord == null)
            {
                return;
            }

            Translation undefinedTranslations = translatedWord.OtherTranslations.SingleOrDefault(x => x.Type == 2);

            if (undefinedTranslations == null)
            {
                return; 
            }

            IEnumerable<string> definedTranslations =
                translatedWord.OtherTranslations.Where(x => x.Type != 2).SelectMany(x => x.Translations);

            undefinedTranslations.Translations =
                undefinedTranslations.Translations.Except(definedTranslations, StringComparer.OrdinalIgnoreCase)
                    .ToList();

            if (!undefinedTranslations.Translations.Any())
            {
                ((List<Translation>)translatedWord.OtherTranslations).Remove(undefinedTranslations);
            }
        }



        #region Speech

        public static Dictionary<Language, IEnumerable<SpeechType>> GetSpeechTypes()
        {
            Dictionary<Language, IEnumerable<SpeechType>> types = new Dictionary<Language, IEnumerable<SpeechType>>();

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.Text,
                    "SELECT * FROM [dbo].[VW_SPEECH_Voices];"))
            {
                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.Dictionary,
                    reader =>
                    {
                        int lastLanguage = 0,
                            lastType = 0;

                        SpeechType speechType = null;

                        while (reader.Read())
                        {
                            int language = (int)reader["language"],
                                type = (int)reader["type_id"];

                            Language languageType = language.ParseWordLanguageDatabaseId();

                            if (language != lastLanguage)
                            {
                                lastLanguage = language;
                                lastType = 0;

                                types.Add(languageType, new List<SpeechType>());
                            }

                            if (type != lastType)
                            {
                                speechType = new SpeechType
                                {
                                    Id = type,
                                    Name = (string)reader["type_name"],
                                    Description = reader["type_description"] as string,
                                    Position = (int)reader["type_position"],
                                    Default = (bool)reader["type_default"],
                                    Selectable = (bool)reader["type_selectable"],
                                    Image = reader["type_image"] as string,
                                    Voices = new List<SpeechVoice>()
                                };

                                ((List<SpeechType>)types[languageType]).Add(speechType);
                            }

                            ((List<SpeechVoice>)speechType.Voices).Add(new SpeechVoice
                            {
                                Id = (int)reader["voice_id"],
                                Name = (string)reader["voice_name"],
                                Description = reader["voice_description"] as string,
                                Position = (int)reader["voice_position"],
                                Default = (bool)reader["voice_default"],
                                Selectable = (bool)reader["voice_selectable"],
                                Image = reader["voice_image"] as string
                            });
                        }
                    });
            }

            return types;
        }


        public static byte[] GetSpeech(Language language, int voice, string data, Func<string> getId, Func<string> getSecret)
        {
            byte[] audio;

            if (DictionaryService.SpeechExists(language, data))
            {
                using (
                    SqlCommand command = SqlDatabaseManager.CreateCommand(
                        CommandType.StoredProcedure,
                        "[dbo].[SPEECH_Get]"))
                {
                    SqlDatabaseManager.AddParameter(
                        command,
                        "@language",
                        ParameterDirection.Input,
                        SqlDbType.Int,
                        language.GetDatabaseId());

                    SqlDatabaseManager.AddParameter(
                        command,
                        "@voice",
                        ParameterDirection.Input,
                        SqlDbType.Int,
                        voice);

                    SqlDatabaseManager.AddParameter(
                        command,
                        "@data",
                        ParameterDirection.Input,
                        SqlDbType.NVarChar,
                        data);

                    audio = SqlDatabaseManager.ExecuteScalar<byte[]>(command, SqlConnectionString.Dictionary);
                }
            }
            else
            {
                string id = getId(), secret = getSecret();
                audio = InsertTextToSpeech(id, secret, language, data);
            }

            return audio;
        }

        private static bool SpeechExists(Language language, string data)
        {
            bool exists;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[SPEECH_Exists]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@data",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    data);

                exists = SqlDatabaseManager.ExecuteScalar<bool>(command, SqlConnectionString.Dictionary);
            }

            return exists;
        }

        private static byte[] InsertTextToSpeech(string id, string secret, Language language, string word)
        {
            MicrosoftSpeak microsoftSpeak = new MicrosoftSpeak(id, secret, language.GetPrefix(LanguagePrefixType.MicrosoftSpeak))
            {
                Format = StreamFormat.MP3,
                Quality = StreamQuality.MinSize
            };

            byte[] audio = microsoftSpeak.GetBytes(word);
            Task.Factory.StartNew(() =>
            {
                DictionaryInsertSpeechModel model = new DictionaryInsertSpeechModel
                    {
                        Language = language,
                        Word = word,
                        Audio = audio
                    };

                DictionaryService.Execute("InsertTextToSpeech", model);
            });

            return audio;
        }

        #endregion
    }
}
