namespace Heracles.Web.Security
{
    using System;
    using System.Collections.Generic;
    using System.Web.Routing;

    public static class AlteaModuleRouteDictionary
    {
        private static readonly Dictionary<string, AlteaModuleRoute> modules =
            new Dictionary<string, AlteaModuleRoute>()
                {
                    // DEVELOPER MODULES
                    {
                        "CACHE",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = string.Empty,
                                        controller = "Cache"
                                    }),
                                Visible = true,
                                StatsVisible = false,
                                HasController = false,
                                HasStats = false,
                                HasLevel = false,
                                HasProLevel = false
                            }
                    },
                    {
                        "CLEAR CACHE",
                        new AlteaModuleRoute
                            {
                                Action = "Clear",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = string.Empty,
                                        controller = "Cache"
                                    }),
                                Visible = true,
                                StatsVisible = false,
                                HasController = false,
                                HasStats = false,
                                HasLevel = false,
                                HasProLevel = false
                            }
                    },
                    {
                        "CLEAR SETTINGS",
                        new AlteaModuleRoute
                            {
                                Action = "Clear",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = string.Empty,
                                        controller = "Settings"
                                    }),
                                Visible = true,
                                StatsVisible = false,
                                HasController = false,
                                HasStats = false,
                                HasLevel = false,
                                HasProLevel = false
                            }
                    },

                    // ADMIN MODULES
                    {
                        "ADMIN",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = "Admin",
                                        controller = "Index"
                                    }),
                                Visible = true,
                                StatsVisible = false,
                                HasController = true,
                                HasStats = false,
                                HasLevel = false,
                                HasProLevel = false
                            }
                    },
                    {
                        "VIEWER",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = string.Empty,
                                        controller = "Viewer"
                                    }),
                                Visible = true,
                                StatsVisible = false,
                                HasController = true,
                                HasStats = false,
                                HasLevel = false,
                                HasProLevel = false
                            }
                    },

                    // TEACHER MODULES
                    {
                        "DEAN",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = "Dean",
                                        controller = "Index"
                                    }),
                                Visible = true,
                                StatsVisible = false,
                                HasController = true,
                                HasStats = false,
                                HasLevel = false,
                                HasProLevel = false
                            }
                    },
                    {
                        "TEACHER",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = "Teacher",
                                        controller = "Index"
                                    }),
                                Visible = true,
                                StatsVisible = false,
                                HasController = true,
                                HasStats = false,
                                HasLevel = false,
                                HasProLevel = false
                            }
                    },

                    // USER MODULES
                    {
                        "WISENET",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = "WiseNet",
                                        controller = "Browser"
                                    }),
                                Visible = true,
                                StatsVisible = true,
                                HasController = true,
                                HasStats = false,
                                HasLevel = false,
                                HasProLevel = false
                            }
                    },
                    {
                        "WISEREADER",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = "WiseReader",
                                        controller = "Index"
                                    }),
                                Visible = true,
                                StatsVisible = true,
                                HasController = true,
                                HasStats = false,
                                HasLevel = false,
                                HasProLevel = false
                            }
                    },
                    {
                        "WISETANK",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = "WiseTank",
                                        controller = "Index"
                                    }),
                                Visible = true,
                                StatsVisible = true,
                                HasController = true,
                                HasStats = false,
                                HasLevel = false,
                                HasProLevel = false
                            }
                    },
                    {
                        "WORDSTAX",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = string.Empty,
                                        controller = "WordStax"
                                    }),
                                Visible = true,
                                StatsVisible = true,
                                HasController = true,
                                HasStats = true,
                                HasLevel = false,
                                HasProLevel = false
                            }
                    },
                    {
                        "GRAMSTAX",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = string.Empty,
                                        controller = "GramStax"
                                    }),
                                Visible = true,
                                StatsVisible = true,
                                HasController = true,
                                HasStats = true,
                                HasLevel = false,
                                HasProLevel = false
                            }
                    },
                    {
                        "TERMSTAX",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = string.Empty,
                                        controller = "TermStax"
                                    }),
                                Visible = true,
                                StatsVisible = true,
                                HasController = true,
                                HasStats = true,
                                HasLevel = false,
                                HasProLevel = false
                            }
                    },
                    {
                        "DESKSINDEX",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = "Desks",
                                        controller = "Index"
                                    }),
                                Visible = true,
                                StatsVisible = true,
                                HasController = true,
                                HasStats = true,
                                HasLevel = true,
                                HasProLevel = false
                            }
                    },
                    {
                        "DESKSEXAMS",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = "Desks",
                                        controller = "Exams"
                                    }),
                                Visible = true,
                                StatsVisible = true,
                                HasController = true,
                                HasStats = true,
                                HasLevel = true,
                                HasProLevel = false
                            }
                    },
                    {
                        "DESKSBOOKS",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = "Desks",
                                        controller = "Books"
                                    }),
                                Visible = true,
                                StatsVisible = true,
                                HasController = true,
                                HasStats = true,
                                HasLevel = false,
                                HasProLevel = false
                            }
                    },
                    {
                        "EXTRA",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = "Desks",
                                        controller = "Extra"
                                    }),
                                Visible = true,
                                StatsVisible = true,
                                HasController = true,
                                HasStats = true,
                                HasLevel = false,
                                HasProLevel = false
                            }
                    },
                    {
                        "PRODESKS",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new
                                    {
                                        area = "ProDesks",
                                        controller = "Index"
                                    }),
                                Visible = true,
                                StatsVisible = true,
                                HasController = true,
                                HasStats = true,
                                HasLevel = false,
                                HasProLevel = true
                            }
                    },
                    {
                        "ACHIEVEMENTS",
                        new AlteaModuleRoute
                            {
                                Action = "Index",
                                RouteValues = new RouteValueDictionary(new 
                                    {
                                        area = string.Empty,
                                        controller = "Achievements"
                                    }),
                                Visible = false,
                                StatsVisible = false,
                                HasController = true,
                                HasStats = false,
                                HasLevel = false,
                                HasProLevel = false
                            }
                    }
                };

        public static AlteaModuleRoute GetRoute(string module)
        {
            AlteaModuleRoute route;
            modules.TryGetValue(module.ToUpperInvariant(), out route);

            return route;
        }

        private static RouteValueDictionary GetRouteValues(string module)
        {
            AlteaModuleRoute route = GetRoute(module);
            if (route == null)
            {
                return null;
            }

            RouteValueDictionary routeValues = new RouteValueDictionary(route.RouteValues);
            return routeValues;
        }

        public static RouteValueDictionary GetRouteValues(string module, string action)
        {
            RouteValueDictionary routeValueDictionary = GetRouteValues(module);
            if (routeValueDictionary == null)
            {
                return null;
            }

            if (routeValueDictionary.ContainsKey("action"))
            {
                routeValueDictionary["action"] = action;
            }
            else
            {
                routeValueDictionary.Add("action", action);
            }

            return routeValueDictionary;
        }

        public static RouteValueDictionary GetRouteValues(string module, string action, string controller)
        {
            RouteValueDictionary routeValueDictionary = GetRouteValues(module, action);
            if (routeValueDictionary == null)
            {
                return null;
            }

            if (routeValueDictionary.ContainsKey("controller"))
            {
                routeValueDictionary["controller"] = controller;
            }
            else
            {
                routeValueDictionary.Add("controller", controller);
            }

            return routeValueDictionary;
        }

        public static RouteValueDictionary GetRouteValues<T>(string module, string action, string controller, T id)
        {
            RouteValueDictionary routeValueDictionary = GetRouteValues(module, action, controller);
            if (routeValueDictionary == null)
            {
                return null;
            }

            if (routeValueDictionary.ContainsKey("id"))
            {
                routeValueDictionary["id"] = (object)id;
            }
            else
            {
                routeValueDictionary.Add("id", (object)id);
            }

            return routeValueDictionary;
        }
    }
}