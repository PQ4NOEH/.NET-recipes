namespace Heracles.Services
{
    using System;

    using Altea.Contracts;
    using Altea.Models.Desks;

    public abstract partial class DesksService : Service<IDesksChannel>
    {
        public static void AutoUnblock(Guid user, int level, int days)
        {
            DesksAutoUnblockModel model = new DesksAutoUnblockModel
                {
                    User = user,
                    Level = level,
                    Days = days
                };

            DesksService.Execute("AutoUnblock", model);
        }

        public static void CheckLastBlock(Guid user, int level, int days)
        {
            DesksCheckLastBlockModel model = new DesksCheckLastBlockModel
                {
                    User = user,
                    Level = level,
                    Days = days
                };

            DesksService.Execute("CheckLastBlock", model);
        }
    }
}
