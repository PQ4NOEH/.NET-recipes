namespace Heracles.Web.Controllers
{
    using System;
    using System.Web.Mvc;

    using Altea.Classes.Achievements;

    using Heracles.Models;
    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth]
    public class AchievementsController : AlteaController
    {
        // GET: /Achievements
        [AlteaAuth(Modules = "Achievements")]
        [HttpGet]
        public ActionResult Index()
        {
            return new EmptyResult();
        }

        // POST: /Achievements/Get
        [HttpPost]
        [OnlyAjax]
        public ActionResult Get()
        {
            //AchievementsModel model = new AchievementsModel
            //{
            //    Achievements = AchievementsService.GetAchievements(UserData.LanguageFrom),
            //    UserAchievements = AchievementsService.GetUserAchivements(UserData, UserData.LanguageFrom, UserData.LanguageTo)
            //};

            //return JsonNet(model);

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult Unlock(int achievement, int level)
        {
            //bool unlocked = AchievementsService.UnlockAchievement(
            //    UserData,
            //    UserData.LanguageFrom,
            //    UserData.LanguageTo,
            //    achievement,
            //    level
            //);

            //UserAchievement userAchievement;

            //if (unlocked)
            //{
            //    userAchievement = new UserAchievement
            //    {
            //        Achievement = achievement,
            //        Level = level,
            //        UnlockDate = DateTime.UtcNow
            //    };
            //}
            //else
            //{
            //    userAchievement = null;
            //}

            //AchievementUnlockedModel model = new AchievementUnlockedModel
            //{
            //    Status = unlocked,
            //    Achievement = userAchievement
            //};

            //return JsonNet(model);

            return new EmptyResult();
        }
    }
}