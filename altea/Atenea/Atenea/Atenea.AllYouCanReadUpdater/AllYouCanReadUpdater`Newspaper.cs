namespace Atenea.AllYouCanReadUpdater
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using CsQuery;
    using CsQuery.Implementation;

    public partial class AllYouCanReadUpdater
    {
        private static readonly Uri NewspapersUri = new Uri("http://www.allyoucanread.com/newspapers/");

        private void NewspaperRun()
        {
            CQ indexDocument;
            if (!AllYouCanReadUpdater.LoadUrl(NewspapersUri, out indexDocument))
            {
                return;
            }

            IEnumerable<Uri> stateUris = AllYouCanReadUpdater.NewspapersGetStates(indexDocument);
            HashSet<Newspaper> allNewspapers = new HashSet<Newspaper>();

            foreach (Uri stateUri in stateUris)
            {
                try
                {
                    IEnumerable<Newspaper> newspapers = AllYouCanReadUpdater.NewspaperGetStateNewspapers(
                        stateUri,
                        allNewspapers);

                    allNewspapers.UnionWith(newspapers);
                }
                catch
                {
                    // ignored
                }
            }

            foreach (Newspaper newspaper in allNewspapers)
            {
                try
                {
                    bool status = this.NewspaperSave(newspaper);

                    if (!status && this.Message != null)
                    {
                        this.Message(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Could not save newspaper {0}",
                                newspaper.Name));
                    }
                }
                catch 
                {
                    // ignored
                }
            }
        }

        private static IEnumerable<Uri> NewspapersGetStates(CQ document)
        {
            CQ states = document.Select("select option");

            return
                states.Cast<HTMLOptionElement>()
                    .TakeWhile(option => option.Value != string.Empty)
                    .Select(option => new Uri(option.Value))
                    .ToArray();
        }

        private static IEnumerable<Newspaper> NewspaperGetStateNewspapers(Uri uri, HashSet<Newspaper> newspapers)
        {
            CQ stateDocument;
            if (!AllYouCanReadUpdater.LoadUrl(uri, out stateDocument))
            {
                return null;
            }

            if (stateDocument.Select(".country-newspapers select").Length == 0)
            {

            }
            else
            {
                
            }

            return Enumerable.Empty<Newspaper>();
        }

        private bool NewspaperSave(Newspaper newspaper)
        {
            return true;
        }
    }
}
