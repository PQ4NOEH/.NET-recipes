namespace Heracles.Web.Areas.WiseNet.Controllers
{
    using System;
    using System.Text;
    using System.Web.Mvc;

    using Altea.Classes.WiseNet;

    using Heracles.Models.WiseNet;
    using Heracles.Net;
    using Heracles.Services;
    using Heracles.Web.ActionFilters;

    [AlteaAuth(Modules = "WiseNet")]
    public class BrowserController : AlteaController
    {
        // GET: /WiseNet/{url}
        [HttpGet]
        public ActionResult Index()
        {
            WiseNetModel model = new WiseNetModel
                {
                    DefaultSearchUrl =
                        WiseNetService.GetDefaultSearchEngine(
                            this.AlteaUser.Id,
                            this.AlteaUser.From)
                };

            return this.View("WiseNetLayout", model);
        }

        // GET | POST: /WiseNet/Load/{parser}
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Load(HtmlParser parser, string uri)
        {
            #if DEBUG
                // ReSharper disable once InconsistentNaming
                const bool isDeveloper = true;
            #else
                bool isDeveloper = !RoleProvider.IsUserInRole(this.AlteaUser.Name, "Developer");
                if (parser != HtmlParser.Default && !isDeveloper)
                {
                    return new HttpNotFoundResult();
                }
            #endif

            if (string.IsNullOrEmpty(uri) || uri.Length % 4 != 0)
            {
                throw new ArgumentException("A valid URI must be provided.", "uri");
            }

            string decodedUrl;
            try
            {
                decodedUrl =
                    Encoding.Default.GetString(Convert.FromBase64String(uri.Replace('-', '=').Replace('_', '/')));
            }
            catch
            {
                throw new ArgumentException("A valid URI must be provided.", "uri");
            }

            UriBuilder uriBuilder = new UriBuilder(decodedUrl) { Fragment = string.Empty };
            HttpMethod method = HttpMethodConverter.Convert(this.Request.HttpMethod);

            if (method == HttpMethod.Post)
            {
                HttpMethod formMethod = HttpMethodConverter.Convert(this.Request.QueryString["method"]);
                method = formMethod == HttpMethod.NotSupported ? HttpMethod.Get : formMethod;
            }

            if (uriBuilder.Scheme == "wisenet")
            {
                return this.LoadInternal(uriBuilder.Uri);
            }

            WiseNetDocument document;

            try
            {
                document = WiseNetService.GetDocument(parser, uriBuilder.Uri, method, this.Request, isDeveloper);
            }
            catch (Exception e)
            {
                // Se ha producido un error en el parseo. Hemos de guardar la excepción en algun parte
                // para poder analizarla y arreglar el problema. Además, mostraremos una pantalla de error
                // informando al usuario de que esa página no está disponible y que estamos trabajando en ello.
                WiseNetException wne = new WiseNetException(decodedUrl, e);
                throw wne;
            }

            this.Response.StatusCode = (int)document.StatusCode;
            this.Response.StatusDescription = document.StatusDescription;

            if (document.ContentType.StartsWith("text/html", StringComparison.InvariantCultureIgnoreCase))
            {
                return this.Content(document.Document, document.ContentType);
            }

            if (document.ContentType.StartsWith("text/css", StringComparison.InvariantCultureIgnoreCase))
            {
                return this.Content(document.Document, document.ContentType);
            }

            // If Content-Type not supported, stream URL response
            return new FileStreamResult(
                document.DocumentStream,
                string.IsNullOrWhiteSpace(document.ContentType) ? "application/octet-stream" : document.ContentType);
        }

        // POST: /WiseNet/ReportError
        [HttpPost, OnlyAjax]
        public ActionResult ReportError(string uri)
        {
            // TODO: Store in database
            WiseNetException wne = new WiseNetException(uri, null);
            throw wne;
        }

        private ActionResult LoadInternal(Uri uri)
        {
            switch (uri.Host) {
                case "home":
                    return this.LoadHome();

                default:
                    return new HttpNotFoundResult();
            }
        }

        private ActionResult LoadHome()
        {
            WiseNetLandingModel model = new WiseNetLandingModel
                {
                    SearchEngines = WiseNetService.GetUserSearchEngines(this.AlteaUser.Id, this.AlteaUser.From),
                    Magazines = WiseNetService.GetUserMagazines(this.AlteaUser.Id, this.AlteaUser.From)
                };

            return this.View("WiseNetLanding", model);
        }
    }
}