using System;
using TheMenu.Core;

namespace Business
{
    public class ImageChanged : ChangeImage, IEvent
    {
        public readonly Guid AggregateId;
        public ImageChanged(Guid aggregateId, byte[] image) 
            : base(image)
        {
            AggregateId = aggregateId;
        }
    }
}
