namespace Heracles.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Altea.Classes;
    using Altea.Classes.Stax;
    using Altea.Classes.Stax.WordStax;
    using Altea.Classes.WiseLab;
    using Altea.Extensions;

    using Heracles.Models.Stax;
    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Modules = "WordStax")]
    public class WordStaxController : AlteaController
    {

        // GET: /Vocabulary
        [HttpGet]
        public ActionResult Index()
        {
            return this.View("Index");
        }

        #region Inbox

        // POST: /WordStax/Inbox
        [HttpPost]
        [OnlyAjax]
        public ActionResult Inbox()
        {
            IEnumerable<WordStaxInboxData> inbox = WordStaxService.GetInbox(this.AlteaUser.Id, this.AlteaUser.From);
            // ReSharper disable once PossibleMultipleEnumeration
            string[] words = inbox.Select(x => x.Data).ToArray();

            Dictionary<string, TranslatedWord> translations =
                DictionaryService.TranslateWords(words, this.AlteaUser.From, this.AlteaUser.To)
                    .ToDictionary(x => x.Word, x => x);

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (WordStaxInboxData data in inbox)
            {
                TranslatedWord wordTranslations;
                translations.TryGetValue(data.Data.ToLowerInvariant(), out wordTranslations);
                data.Translations = wordTranslations;
            }

            IDictionary<string, IEnumerable<string>> wordsInStax = WordStaxService.CheckWordsInStax(
                this.AlteaUser.Id,
                this.AlteaUser.From,
                this.AlteaUser.To,
                words);

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (WordStaxInboxData data in inbox)
            {
                IEnumerable<string> dataInStacks;
                wordsInStax.TryGetValue(data.Data, out dataInStacks);
                data.DataInStax = dataInStacks;
            }

            StaxInboxModel model = new StaxInboxModel
                {
                    Inbox = inbox,
                    DataInStax = wordsInStax
                };

            return this.JsonNet(model);
        }

        // POST: /WordStax/Search
        [HttpPost]
        [OnlyAjax]
        public ActionResult Search(string word, int offsetDate)
        {
            WiseLabError error = WiseLabError.None;
            int id = 0;
            TranslatedWord translatedWord = DictionaryService.TranslateWord(
                word.ToLowerInvariant(),
                this.AlteaUser.From,
                this.AlteaUser.To);

            if (translatedWord == null)
            {
                error = WiseLabError.HuntDataNotExists;
            }
            else
            {
                id = StaxService.InsertInboxData(
                    this.AlteaUser.Id,
                    this.AlteaUser.From,
                    StackType.Vocabulary,
                    WordStaxOrigin.Search,
                    null,
                    word,
                    true,
                    null,
                    Convert.ToInt32(this.AlteaUser["vocabulary_inbox_overflow"]),
                    offsetDate);

                if (id <= 0)
                {
                    translatedWord = null;
                }
            }

            WordStaxSearchModel model = new WordStaxSearchModel
            {
                Error = error,
                Id = id,
                Translations = translatedWord
            };

            return this.JsonNet(model);
        }

        // POST: /WordStax/Accept
        [HttpPost]
        [OnlyAjax]
        public ActionResult Accept(long id, string word, string[] translations, int[] types, int offsetDate)
        {
            int maxData = Convert.ToInt32(this.AlteaUser["vocabulary_inbox_max_data"]);
            int translationsCount = translations.Select(x => x.Trim().ToLowerInvariant()).Distinct().Count();
            if (translationsCount == 0 || translationsCount > maxData || translationsCount != types.Length)
            {
                return this.JsonNet(AlteaError.UnknownError);
            }

            TranslatedWord translatedWord = DictionaryService.TranslateWord(
                word.ToLowerInvariant(),
                this.AlteaUser.From,
                this.AlteaUser.To);

            IEnumerable<string> invalidTranslations =
                translations.Select(x => x.Trim()).Except(
                    translatedWord.OtherTranslations.SelectMany(x => x.Translations)
                        .Union(Enumerable.Repeat(translatedWord.SuggestedTranslation, 1)),
                    StringComparer.InvariantCultureIgnoreCase);

            if (invalidTranslations.Count() != 0)
            {
                return this.JsonNet(AlteaError.UnknownError);
            }
            
            KeyValuePair<string, IEnumerable<string>> wordInStax = WordStaxService.CheckWordsInStax(
                this.AlteaUser.Id,
                this.AlteaUser.From,
                this.AlteaUser.To,
                new string[] { word }).SingleOrDefault(x => x.Key.Equals(word.Trim(), StringComparison.InvariantCultureIgnoreCase));

            if (!wordInStax.Equals(default(KeyValuePair<string, IEnumerable<string>>))
                && wordInStax.Value.Select(x => x.Trim())
                       .Intersect(translations.Select(x => x.Trim()), StringComparer.InvariantCultureIgnoreCase)
                       .Any())
            {
                return this.JsonNet(AlteaError.UnknownError);
            }

            StaxContentData[] dataOptions = new StaxContentData[translationsCount];

            for (int i = 0; i < translationsCount; i++)
            {
                dataOptions[i] = new StaxContentData
                    {
                        Data = translations[i],
                        Type = types[i]
                    };
            }

            StaxService.AcceptInboxData(
                this.AlteaUser.Id,
                this.AlteaUser.From,
                this.AlteaUser.To,
                StackType.Vocabulary,
                id,
                word,
                maxData,
                dataOptions,
                offsetDate);

            return this.JsonNet(AlteaError.NoError);
        }

        [HttpPost]
        [OnlyAjax]
        public ActionResult Delete(int id, string word)
        {
            StaxService.DeleteInboxData(this.AlteaUser.Id, this.AlteaUser.From, StackType.Vocabulary, id, word);
            return new EmptyResult();
        }

        #endregion

        #region Graphs

        [HttpPost]
        [OnlyAjax]
        public ActionResult IndexGraphs(int offsetDate)
        {
            IEnumerable<StaxWeekGraph> graphs = StaxService.GetWeekGraphs(
                this.AlteaUser.Id,
                this.AlteaUser.From,
                this.AlteaUser.To,
                StackType.Vocabulary,
                new string[] { "steps", "finished" },
                DateTime.UtcNow.AddMinutes(-offsetDate),
                Convert.ToInt32(this.AlteaUser["week_start_day"]),
                offsetDate);

            return this.JsonNet(graphs);
        }

        #endregion

        #region Stacks

        [HttpGet]
        public ActionResult Stack(int id)
        {
                if (id < WordStaxService.MinStack || id > WordStaxService.MaxStack)
                {
                    return this.RedirectToAction("Index");
                }

                StaxStackModel model = new StaxStackModel
                    {
                        StackNum = id,
                        MaxStack = WordStaxService.MaxStack,
                        TimeLimit = Convert.ToInt32(this.AlteaUser["vocabulary_stack_" + id + "_timelimit"])
                    };

                return this.View("Stack", model);
        }

        // GET: /WordStax/Finished
        public ActionResult Finished()
        {
            return null;
        }

        // POST: /WordStax/Stacks
        [HttpPost]
        [OnlyAjax]
        public ActionResult Stax()
        {
            StaxService.CheckStaxExist(
                this.AlteaUser.Id,
                this.AlteaUser.From,
                this.AlteaUser.To,
                StackType.Vocabulary,
                WordStaxService.MaxStack);

            IEnumerable<Stack> stax = WordStaxService.GetStax(this.AlteaUser.Id, this.AlteaUser.From, this.AlteaUser.To);
            int finished = WordStaxService.CountFinishedData(this.AlteaUser.Id, this.AlteaUser.From, this.AlteaUser.To);
            StackSettings settings = new StackSettings
                {
                    Underflow = Convert.ToInt32(this.AlteaUser["vocabulary_stax_underflow"]),
                    Overflow = Convert.ToInt32(this.AlteaUser["vocabulary_stax_overflow"]),
                    NumStax = WordStaxService.MaxStack,
                    Steps = Convert.ToInt32(this.AlteaUser["vocabulary_stax_steps"])
                };

            StaxModel model = new StaxModel { Stax = stax, Finished = finished, Settings = settings };
            return this.JsonNet(model);
        }

        #endregion

        #region Exercises

        // POST: /WordStax/New/{id}
        [HttpPost]
        [OnlyAjax]
        public ActionResult New(int id, long[] excluded, bool checkExcluded)
        {
            if (id < WordStaxService.MinStack || id > WordStaxService.MaxStack)
            {
                throw new NotImplementedException();
            }

            StaxExerciseModel model = new StaxExerciseModel();

            long[] excludedData = checkExcluded
                ? excluded ?? Enumerable.Empty<long>().ToArray()
                : Enumerable.Empty<long>().ToArray();

            if (WordStaxService.CheckStackUnderflow(
                this.AlteaUser,
                this.AlteaUser.From,
                this.AlteaUser.To,
                id,
                excludedData))
            {
                model.Status = StackStatus.StackUnderflow;
                return this.JsonNet(model);
            }

            if (id < WordStaxService.MaxStack
                && WordStaxService.CheckStackOverflow(
                    this.AlteaUser,
                    this.AlteaUser.From,
                    this.AlteaUser.To,
                    id + 1,
                    excludedData))
            {
                model.Status = StackStatus.StackOverflow;
                return this.JsonNet(model);
            }

            int numberData = Convert.ToInt32(this.AlteaUser["vocabulary_stack_" + id + "_number_data"]);
            int extraData = Convert.ToInt32(this.AlteaUser["vocabulary_stack_" + id + "_extra_data"]);
            int maxData = Convert.ToInt32(this.AlteaUser["vocabulary_stack_" + id + "_max_data"]);

            IStackFormula formula = WordStaxService.NewExercise(
                this.AlteaUser.Id,
                AppCore.AppId,
                this.AlteaUser.From,
                this.AlteaUser.To,
                id,
                numberData,
                extraData,
                maxData,
                excludedData);

            model.Formula = formula;

            return this.JsonNet(model);
        }

        // POST: /WordStack/Save/{id}
        [HttpPost]
        [OnlyAjax]
        public ActionResult Save(
            int id,
            int[] ids,
            string[][] contents,
            bool status,
            double time,
            int offsetDate)
        {
            if (id < WordStaxService.MinStack || id > WordStaxService.MaxStack)
            {
                throw new NotImplementedException();
            }

            StackExerciseAnswer[] answers = new StackExerciseAnswer[ids.Count()];

            for (int i = 0, l = ids.Count(); i < l; i++)
            {
                answers[i] = new StackExerciseAnswer { Id = ids[i], Answers = contents[i] };
            }

            IEnumerable<string> inbox = StaxService.SaveExercise(
                this.AlteaUser.Id,
                AppCore.AppId,
                !AppCore.IsLocal(this.Request.GetIpAddress()),
                this.AlteaUser.From,
                this.AlteaUser.To,
                StackType.Vocabulary,
                id,
                answers,
                status,
                TimeSpan.FromSeconds(time),
                Convert.ToInt32(this.AlteaUser["vocabulary_stack_retry_cooldown"]),
                Convert.ToInt32(this.AlteaUser["vocabulary_stack_inbox_errors"]),
                offsetDate);

            return this.JsonNet(inbox);
        }

        #endregion
    }
}