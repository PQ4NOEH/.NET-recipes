using TheMenu.Core;

namespace Business
{
    public class ChangeImage : ICommand
    {
        public readonly byte[] Image;
        public ChangeImage(byte[] image)
        {
            Image = image;
        }
    }
}
