using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altea.Classes.WiseNet
{
    using System.IO;
    using System.Net;

    public class WiseNetDocument
    {
        public Uri Url { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string ContentType { get; set; }
        public string Document { get; set; }
        public Stream DocumentStream { get; set; }
    }
}
