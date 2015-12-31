using AlteaLabs.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlteaLabs.WiseLab.Contracts
{
    public interface IWiseLabService
    {
        WiseLabArticle GetArticle(Guid userId, Language from, Language to, WiseLabOrigin origin, int reference);
        WiseLabError SearchWord(Guid userId, Language from, Language to, WiseLabOrigin origin, int reference, string data, int inboxOverflow, int offsetDate);
        WiseLabError AddHuntData(Guid userId, Language from, Language to, WiseLabOrigin origin, int reference, string data, string sentence, int inboxOverflow, int offsetDate);
        WiseLabError RemoveHuntData(Guid userId, Language from, Language to, WiseLabOrigin origin, int reference, string data);
        WiseLabError SaveLead( Guid userId, Language from, Language to, WiseLabOrigin origin, int reference, string lead, bool autoSave);
        WiseLabStatus FinishStatus(Guid userId, Language from, Language to, WiseLabOrigin origin, int reference, int offsetDate);


        //WiseLabError AddHuntData(WiseLabHuntDataModel model);
        //WiseLabStatus FinishStatus(WiseLabArticleDataModel model);
        //WiseLabError RemoveHuntData(WiseLabHuntDataModel model);
        //WiseLabError SaveLead(WiseLabWisdomHunterModel model);
        //WiseLabError SearchWord(WiseLabHuntDataModel model);
        
    }
}
