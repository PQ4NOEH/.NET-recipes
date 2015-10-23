using TheMenu.Core;
using TheMenu.Core.Guards;

namespace Business
{
    public class ChangeName : ICommand
    {
        public readonly string NewName;
        public ChangeName(NotNullOrWhiteSpaceString newName)
        {
            NewName = newName;
        }
    }
}
