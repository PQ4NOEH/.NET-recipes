using TheMenu.Core;

namespace Business
{
    public class RemoveStep : ICommand
    {
        public readonly uint StepOrder;
        public RemoveStep(uint stepOrder)
        {
            StepOrder = stepOrder;
        }
    }
}
