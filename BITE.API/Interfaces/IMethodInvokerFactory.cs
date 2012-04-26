using System;
using System.Web;
using System.Web.Routing;

namespace BITE.Server.Plugins.Interfaces {
   public interface IMethodInvoker {
      Object InvokeAdapterMethod(RequestContext request, Object adapter);
      event EventHandler<InvokeAdapterMethodEventArgs> MethodInvoked;
   }
}