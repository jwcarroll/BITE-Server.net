using System;
using System.Web.Routing;
using System.Web;
using BITE.Server.Plugins;
using BITE.Server.Plugins.Interfaces;

namespace BITE.Server {
   public class BiteRequestRouteHandler: IRouteHandler {

      #region IRouteHandler Members

      public IHttpHandler GetHttpHandler(RequestContext requestContext) {
         return new BiteRequestHandler(requestContext,
                                       DependencyResolver.Default.GetService<IAdapterFactory>(),
                                       DependencyResolver.Default.GetService<IMethodInvoker>());
      }

      #endregion
   }
}
