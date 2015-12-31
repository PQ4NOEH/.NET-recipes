namespace Heracles.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Altea.Classes.Stats;

    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth]
    public class StatsController : AlteaController
    {
        [HttpPost, OnlyAjax]
        public ActionResult Get(string[] modules)
        {
            ModuleStats[] stats =
                StatsService.GetStats(
                    this.AlteaUser.Id,
                    AppCore.AppId,
                    this.AlteaUser.From,
                    this.AlteaUser.To,
                    this.AlteaUser.Level == null ? (int?)null : this.AlteaUser.Level.Id,
                    this.AlteaUser.ProLevel == null ? (int?)null : this.AlteaUser.ProLevel.Id,
                    this.AlteaUser.ProLevel == null ? (int?)null : this.AlteaUser.ProLevel.SubId,
                    modules)
                .ToArray();

            IEnumerable<ModuleStats> noStatusStats =
                stats.Where(x => x.Stats.Any(y => y.Status == ModuleStatsStatus.NoStatus));

            IEnumerable<ModuleStatsData> levelStats =
                stats.Where(x => x.Stats.Any(y => y.Name == "level"))
                    .Select(x => x.Stats.SingleOrDefault(z => z.Name == "level"));

            IEnumerable<ModuleStatsData> proLevelStats =
                stats.Where(x => x.Stats.Any(y => y.Name == "prolevel"))
                    .Select(x => x.Stats.SingleOrDefault(z => z.Name == "prolevel"));
            
            foreach (ModuleStatsData data in levelStats)
            {
                data.Value = this.AlteaUser.Level == null ? string.Empty : this.AlteaUser.Level.UserDisplayName;
            }

            foreach (ModuleStatsData data in proLevelStats)
            {
                data.Value = this.AlteaUser.ProLevel == null
                                 ? string.Empty
                                 : this.AlteaUser.ProLevel.UserDisplaySubName ?? this.AlteaUser.ProLevel.UserDisplayName;
            }

            foreach (ModuleStats module in noStatusStats)
            {
                IEnumerable<string> settings = StatsService.ModuleSettings[module.Name];
                IDictionary<string, string> settingsData = settings.ToDictionary(x => x, x => this.AlteaUser[x]);
                StatsService.SetStatus(module, settingsData);
            }

            return this.JsonNet(stats);
        }
    }
}