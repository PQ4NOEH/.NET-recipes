namespace Heracles.Web
{
    using System.Globalization;
    using System.Web.Mvc;

    using Altea.Classes.Members;
    using Altea.Extensions;

    // Anything that you want passed from a view to the layout goes here
    public class AlteaViewHints
    {
        public bool FullContainer { get; set; }
        public bool HideWiseFooter { get; set; }
    }

    public abstract class AlteaViewPage<TModel> : WebViewPage<TModel>
    {
        private bool? isLocal = null;

        public AlteaViewHints ViewHints
        {
            get
            {
                if (!this.ViewData.ContainsKey("AlteaViewHints"))
                {
                    this.ViewData["AlteaViewHints"] = new AlteaViewHints();
                }

                return (AlteaViewHints)this.ViewData["AlteaViewHints"];
            }
        }

        public User AlteaUser
        {
            get
            {
                return (User)this.ViewData["AlteaViewUser"];
            }
        }

        public CultureInfo CultureInfo
        {
            get
            {
                return (CultureInfo)this.ViewData["CultureInfo"];
            }
        }

        public bool IsLocal
        {
            get
            {
                if (!this.isLocal.HasValue)
                {
                    this.isLocal = AppCore.IsLocal(this.Request.GetIpAddress());
                }

                return this.isLocal.Value;
            }
        }

        public bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}