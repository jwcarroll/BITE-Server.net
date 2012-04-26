using System;
using System.Web;
using BITE.Server.Plugins.Interfaces;
using SignalR.Hubs;
using SignalR;
using SignalR.Hosting.AspNet;
using SignalR.Infrastructure;
using System.Web.Helpers;
using System.Web.Routing;
using BITE.Server.Plugins.Exceptions;

namespace BITE.Server
{
    public class BiteRequestHandler : IHttpHandler
    {
        protected IAdapterFactory AdapterFactory { get; set; }
        protected IMethodInvoker MethodInvoker { get; set; }
        protected RequestContext RequestContext { get; set; }

        public BiteRequestHandler(RequestContext requestContext, IAdapterFactory adapterFactory, IMethodInvoker methodInvoker)
        {
            AssertDependencyIsNotNull(requestContext, "requestContext");
            AssertDependencyIsNotNull(adapterFactory, "adapterFactory");
            AssertDependencyIsNotNull(methodInvoker, "methodInvoker");

            RequestContext = requestContext;
            AdapterFactory = adapterFactory;
            MethodInvoker = methodInvoker;

           MethodInvoker.MethodInvoked += PublishRequestMade;
        }

        public void ProcessRequest(HttpContext context)
        {
            ProcessRequest(new HttpContextWrapper(context));
        }

        public void ProcessRequest(HttpContextBase context)
        {
            WriteResponse(context);
        }

        protected virtual void WriteResponse(HttpContextBase context)
        {
            var adapterName = RequestContext.RouteData.GetRequiredString("adapter");            
            var adapter = GetAdaper(adapterName);

            var returnVal = MethodInvoker.InvokeAdapterMethod(RequestContext, adapter);

            var jsonData = Json.Encode(returnVal);

            context.Response.ContentType = "application/json";
            context.Response.Write(jsonData);
        }

        private object GetAdaper(String adapterName)
        {
            try
            {
                return AdapterFactory.CreateAdapter(adapterName);
            }
            catch (AdapterNotFoundException ex)
            {
                return new MethodNotFoundAdapter();
            }
        }

        public static void PublishRequestMade(object sender, InvokeAdapterMethodEventArgs eventArgs)
        {
            var clients = ConnectionManager.GetClients<BiteRequestHub>();

           var biteRequest = new BiteRequest(eventArgs);

            clients.publishRequestMade(biteRequest);
        }

        private void AssertDependencyIsNotNull<T>(T adapterFactory, String argumentName) where T : class
        {
            if (adapterFactory == null)
            {
                throw new ArgumentNullException(argumentName, String.Format("An instance of {0} must be provided to {1}", typeof(T).Name, GetType().Name));
            }
        }

        private static IConnectionManager _connectionManager;

        private static IConnectionManager ConnectionManager
        {
            get
            {
                return _connectionManager ??
                       (_connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>());
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

       ~BiteRequestHandler() {
          MethodInvoker.MethodInvoked -= PublishRequestMade;
       }
    }
}