using TheMenu.Core;
using TheMenu.Core.Guards;

namespace Business
{
    public class Step: IValueObject
    {
        public readonly uint Order;
        public readonly string Explanation;
        public readonly byte[] Image;
        public Step(uint order, NotNullOrWhiteSpaceString explanation, byte[] image)
        {
            Order = order;
            Explanation = explanation;
            Image = image;
        }
    }
}
