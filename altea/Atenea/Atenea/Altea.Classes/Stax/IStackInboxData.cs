namespace Altea.Classes.Stax
{
    using System;
    using System.Collections.Generic;

    public interface IStackInboxData
    {
        long Id { get; set; }

        int Origin { get; set; }

        DateTime InsertDate { get; set; }

        string Data { get; set; }

        int NumErrors { get; set; }

        bool Reinserted { get; set; }

        IEnumerable<string> DataInStax { get; set; }
    }
}
