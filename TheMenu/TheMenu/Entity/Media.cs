using TheMenu.Core.Guards;

namespace TheMenu.Core.Entity
{
    public abstract class Media
    {
        public byte[] Content { get; private set; }
        public string MediaType { get; private set; }

        public Media(NotNullable<byte[]> content, NotNullOrWhiteSpaceString mediaType)
        {
            Content = content;
            MediaType = mediaType;
        }
    }
}
