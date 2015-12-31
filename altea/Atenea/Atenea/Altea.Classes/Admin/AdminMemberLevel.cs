namespace Altea.Classes.Admin
{
    using Altea.Common.Classes;

    public class AdminMemberLevel
    {
        public Language LanguageFrom { get; set; }

        public Language? LanguageTo { get; set; }

        public int Level { get; set; }
        
        public int? SubLevel { get; set; }

        public bool Primary { get; set; }
    }
}
