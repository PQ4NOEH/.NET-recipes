using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using TheMenu.Core.Guards;

namespace TheMenu.Core.Entity
{
    public class ImageMedia : Media
    {
        public ImageMedia(NotNullable<byte[]> content)
            :base(content, )
    }
}
