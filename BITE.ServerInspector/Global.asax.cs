using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BITE.Server;
using SignalR;
using SignalR.Hosting.AspNet;
using SignalR.Infrastructure;

namespace BITE.ServerInspector {
   // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
   // visit http://go.microsoft.com/?LinkId=9394801

   public class MvcApplication : HttpApplication {
      private static IConnectionManager _connectionManager;

      public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
         filters.Add(new HandleErrorAttribute());
      }

      public static void RegisterRoutes(RouteCollection routes) {
         routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
         
         routes.MapRoute(
             "Default", // Route name
             "Manager/{controller}/{action}/{id}", // URL with parameters
             new { controller = "Inspector", action = "Index", id = UrlParameter.Optional } // Parameter defaults
         );

         routes.Add(new Route(
         "{method}",
         new RouteValueDictionary(new { adapter = "Default", id = UrlParameter.Optional }),
         new RouteValueDictionary(new { method = "check_login_status|get_templates" }),
         new BiteRequestRouteHandler()));

         routes.Add(new Route(
            "{adapter}/{method}/{id}",
            new RouteValueDictionary(new { adapter = "Default", method = UrlParameter.Optional, id = UrlParameter.Optional }),
            new BiteRequestRouteHandler()));
      }

      public override void Init() {
         base.Init();

         //BeginRequest += OnBeginRequest;
      }

      private static void OnBeginRequest(object sender, EventArgs eventArgs) {
         var clients = ConnectionManager.GetClients<BiteRequestHub>();

         var biteRequest = new BiteRequest(new HttpRequestWrapper(HttpContext.Current.Request));

         clients.publishRequestMade(biteRequest);
      }

      protected void Application_Start() {
         AreaRegistration.RegisterAllAreas();

         RegisterGlobalFilters(GlobalFilters.Filters);
         RegisterRoutes(RouteTable.Routes);
      }

      private static IConnectionManager ConnectionManager {
         get {
            return _connectionManager ??
                   (_connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>());
         }
      }
   }
}