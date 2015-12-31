namespace Heracles.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Altea.Classes.Lists;
    using Altea.Common.Classes;
    using Altea.Extensions;

    using Heracles.Models.Lists;
    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth]
    public class ListsController : AlteaController
    {
        // POST: /Lists/Lists
        [HttpPost, OnlyAjax]
        public ActionResult Categories(ListType type)
        {
            if (!Enum.IsDefined(typeof(ListType), type)
                || (type == ListType.All
                    && !RoleProvider.IsUserInModule(
                        this.AlteaUser.Name,
                        "Dean",
                        AppCore.IsLocal(this.Request.GetIpAddress()))))
            {
                throw new ArgumentOutOfRangeException("type");
            }
            
            IEnumerable<ListCategory> categories;

            AlteaCache.GetOrInsert(
                "LISTS_CATEGORIES_" + type + "_" + this.AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName),
                true,
                () => ListsService.GetCategories(this.AlteaUser.From, type),
                AlteaCache.Scope.Altea,
                AlteaCache.Term.Medium,
                out categories);

            return this.JsonNet(categories);
        }

        // POST: /Lists/Lists
        [HttpPost, OnlyAjax]
        public ActionResult Lists(int id)
        {
            if (!Enum.IsDefined(typeof(ListType), id))
            {
                return new HttpNotFoundResult();
            }

            ListType type = (ListType)id;
            IEnumerable<AssignedList> lists = ListsService.GetUserLists(AlteaUser.Id, type, AlteaUser.From);
            IEnumerable<Guid> listIds = lists.Select(x => x.Id);

            //@TODO CACHE
            IEnumerable<ListGroupType> groups = ListsService.GetGroupTypes();
            IDictionary<Guid, AssignedListStatus> status = ListsService.GetUserAssignments(
                AlteaUser.Id,
                type,
                AlteaUser.From,
                listIds);

            List<AssignedList> activeLists = new List<AssignedList>(Math.Min(ListsService.MaxLists, lists.Count()));

            //@TODO CACHE
            Dictionary<int, string> categories = null;
            //bool inCache = AlteaCache.Get(
            //    "LIST_CategoryNames_" + AlteaUser.From.GetPrefix(LanguagePrefixType.ShortName),
            //    AlteaCache.Scope.Altea,
            //    AlteaCache.Term.Largest,
            //    out categories);
            bool inCache = false;
            Dictionary<int, string> categoryNames = null;


            if (!inCache)
            {
                categoryNames = new Dictionary<int, string>(ListsService.MaxLists * 2);
            }

            foreach (AssignedList list in lists)
            {
                AssignedListStatus listStatus;
                status.TryGetValue(list.Id, out listStatus);

                if (listStatus != null
                    && list.DataCount > listStatus.Rejected + listStatus.Finished + listStatus.Recognized)
                {
                    string section, category;
                    if (inCache)
                    {
                        categories.TryGetValue(listStatus.SectionId, out section);
                        categories.TryGetValue(listStatus.CategoryId, out category);
                    }
                    else
                    {
                        if (!categoryNames.TryGetValue(listStatus.SectionId, out section))
                        {
                            section = ListsService.GetCategoryName(listStatus.SectionId, AlteaUser.From);
                            categoryNames.Add(listStatus.SectionId, section);
                        }

                        if (!categoryNames.TryGetValue(listStatus.CategoryId, out category))
                        {
                            category = ListsService.GetCategoryName(listStatus.CategoryId, AlteaUser.From);
                            categoryNames.Add(listStatus.CategoryId, category);
                        }
                    }

                    listStatus.Section = section;
                    listStatus.Category = category;

                    list.Status = listStatus;
                    activeLists.Add(list);
                }
            }

            UserListsModel model = new UserListsModel
                {
                    Groups = groups,
                    Lists = activeLists
                };

            return this.JsonNet(model);
        }
    }
}