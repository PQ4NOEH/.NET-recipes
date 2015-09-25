using System.Collections.Generic;
using System.Text;

namespace AOP.UnitTests
{
    public class OutputToList:IOutput
    {
        public List<string> OutputText = new List<string>();
       

        public void WriteLine(string message)
        {
            OutputText.Add(message);
        }
    }
}
