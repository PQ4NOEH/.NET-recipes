
using TheMenu.Core;
using TheMenu.Core.Guards;

namespace Business
{
    public class AddStep : Step, ICommand
    {
        public AddStep(uint order, NotNullOrWhiteSpaceString explanation)
            : base(order, explanation)
        { }
    }
}
