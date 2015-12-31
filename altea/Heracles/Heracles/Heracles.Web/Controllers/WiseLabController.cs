namespace Heracles.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Altea.Classes.WiseLab;
    using Altea.Common.Classes;
    using Altea.Extensions;

    using Heracles.Models.WiseLab;
    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    using Microsoft.Azure;

    [AlteaAuth]
    public class WiseLabController : AlteaController
    {
        [HttpPost, OnlyAjax]
        public JsonNetResult GetFinishedWords()
        {
            string[] finishedWords = WordStaxService.GetFinishedWords(
                this.AlteaUser.Id,
                this.AlteaUser.From,
                this.AlteaUser.To);

            return this.JsonNet(finishedWords);
        }

        [HttpPost, OnlyAjax]
        public JsonNetResult GetArticle(WiseLabOrigin origin, int reference)
        {
            WiseLabArticle article = WiseLabService.GetArticle(
                this.AlteaUser.Id,
                this.AlteaUser.From,
                this.AlteaUser.To,
                origin,
                reference);

            IEnumerable<WiseLabHuntData> scoutedWords = article.Scouted;

            if (scoutedWords != null)
            {
                IDictionary<string, WiseLabHuntData> scouted = scoutedWords.ToDictionary(
                    x => x.Data,
                    x => x,
                    StringComparer.InvariantCultureIgnoreCase);

                IEnumerable<TranslatedWord> translatedWords =
                    DictionaryService.TranslateWords(scouted.Select(x => x.Key), this.AlteaUser.From, this.AlteaUser.To);

                foreach (TranslatedWord word in translatedWords)
                {
                    scouted[word.Word].Translations = word;
                }
            }

            WiseLabArticleRequestModel model = new WiseLabArticleRequestModel
                {
                    Article = article,
                    Parameters = origin.GetAttribute<WiseLabPropertyAttribute>()
                };

            return this.JsonNet(model);
        }

        [HttpPost, OnlyAjax]
        public JsonNetResult GetOriginParameters(WiseLabOrigin origin)
        {
            return this.JsonNet(origin.GetAttribute<WiseLabPropertyAttribute>());
        }

        [HttpPost, OnlyAjax]
        public JsonNetResult SearchWord(WiseLabOrigin origin, int reference, string data, int offsetDate)
        {
            WiseLabError error;
            TranslatedWord translatedWord = DictionaryService.TranslateWord(
                data,
                this.AlteaUser.From,
                this.AlteaUser.To);

            if (translatedWord == null)
            {
                error = WiseLabError.HuntDataNotExists;
            }
            else
            {
                error = WiseLabService.SearchWord(
                    this.AlteaUser.Id,
                    this.AlteaUser.From,
                    this.AlteaUser.To,
                    origin,
                    reference,
                    data,
                    Convert.ToInt32(this.AlteaUser["vocabulary_inbox_overflow"]),
                    offsetDate);

                if (error != WiseLabError.None)
                {
                    translatedWord = null;
                }
            }

            WiseLabArticleDataRequestModel model = new WiseLabArticleDataRequestModel { Error = error, Translations = translatedWord };
            return this.JsonNet(model);
        }

        [HttpPost, OnlyAjax]
        public JsonNetResult AddHuntData(
            WiseLabOrigin origin,
            int reference,
            string data,
            string sentence,
            bool translate,
            int offsetDate)
        {
            WiseLabError error = WiseLabError.None;
            TranslatedWord translatedWord = null;

            if (translate)
            {
                translatedWord = DictionaryService.TranslateWord(
                    data,
                    this.AlteaUser.From,
                    this.AlteaUser.To);

                if (translatedWord == null)
                {
                    error = WiseLabError.HuntDataNotExists;
                }
            }

            if (error == WiseLabError.None)
            {
                error = WiseLabService.AddHuntData(
                    this.AlteaUser.Id,
                    this.AlteaUser.From,
                    this.AlteaUser.To,
                    origin,
                    reference,
                    data,
                    sentence,
                    translate ? Convert.ToInt32(this.AlteaUser["vocabulary_inbox_overflow"]) : 0,
                    offsetDate);

                if (error != WiseLabError.None && translate)
                {
                    translatedWord = null;
                }
            }

            WiseLabArticleDataRequestModel model = new WiseLabArticleDataRequestModel
                {
                    Error = error,
                    Translations = translatedWord
                };

            return this.JsonNet(model);
        }

        [HttpPost, OnlyAjax]
        public JsonNetResult RemoveHuntData(WiseLabOrigin origin, int reference, string data)
        {
            WiseLabError error = WiseLabService.RemoveHuntData(
                this.AlteaUser.Id,
                this.AlteaUser.From,
                this.AlteaUser.To,
                origin,
                reference,
                data);

            WiseLabArticleDataRequestModel model = new WiseLabArticleDataRequestModel { Error = error };
            return this.JsonNet(model);
        }

        [HttpPost, OnlyAjax]
        public JsonNetResult SaveLead(WiseLabOrigin origin, int reference, string lead, bool autoSave)
        {
            WiseLabError error = WiseLabService.SaveLead(
                this.AlteaUser.Id,
                this.AlteaUser.From,
                this.AlteaUser.To,
                origin,
                reference,
                lead,
                autoSave);

            WiseLabArticleDataRequestModel model = new WiseLabArticleDataRequestModel { Error = error };
            return this.JsonNet(model);
        }

        [HttpPost, OnlyAjax]
        public JsonNetResult FinishStatus(WiseLabOrigin origin, int reference, int offsetDate)
        {
            WiseLabStatus status = WiseLabService.FinishStatus(
                this.AlteaUser.Id,
                this.AlteaUser.From,
                this.AlteaUser.To,
                origin,
                reference,
                offsetDate);

            WiseLabArticleDataRequestModel model = new WiseLabArticleDataRequestModel
                {
                    Error = status == WiseLabStatus.None ? WiseLabError.UnknownError : WiseLabError.None,
                    Status = status
                };

            return this.JsonNet(model);
        }

        [HttpPost, OnlyAjax]
        public EmptyResult LogException(
            WiseLabOrigin origin,
            int reference,
            string stack,
            string message,
            string word,
            string context,
            string parent)
        {
            Task.Run(
                () =>
                    {
                        WiseLabService.LogException(
                            this.AlteaUser.Id,
                            this.AlteaUser.From,
                            this.AlteaUser.To,
                            origin,
                            reference,
                            stack,
                            message,
                            word,
                            context,
                            parent);
                    });

            return new EmptyResult();
        }

        [HttpGet]
        public FileContentResult Speech(int from, string word)
        {
            Language language = from == 1 ? this.AlteaUser.From : this.AlteaUser.To;

            Dictionary<int, int> speechVoices = this.AlteaUser["speech_voices"].FromJson<Dictionary<int, int>>();
            int type = speechVoices.Single(x => x.Key == language.GetDatabaseId()).Value;

            word = word.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(word) || word.Length > 220)
            {
                throw new ArgumentOutOfRangeException("word");
            }

            // Chrome 39 doesn't replay audios without this header
            this.Response.Headers.Add("Accept-Ranges", "bytes");

            byte[] audio = DictionaryService.GetSpeech(
                language,
                type,
                word,
                () => CloudConfigurationManager.GetSetting("MicrosoftTranslator.Id"),
                () => CloudConfigurationManager.GetSetting("MicrosoftTranslator.Secret"));
            return this.File(audio, "audio/mpeg");
        }
    }
}