namespace Altea.Classes.WiseNet
{
    using System.Collections.Generic;

    public class BlacklistData
    {
        public BlacklistStrength Strength { get; set; }
        public IDictionary<string, string> AddRequestParameters { get; set; }
        public IEnumerable<string> RemoveRequestParameters { get; set; }
        public IEnumerable<string> RemoveTags { get; set; }
        public IEnumerable<string> RemoveAttributes { get; set; }
        public string InjectHeadCode { get; set; }
 
    }
}
