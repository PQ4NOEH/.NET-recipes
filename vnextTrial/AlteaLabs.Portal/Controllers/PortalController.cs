﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlteaLabs.Portal.Controllers
{
    public class PortalController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}