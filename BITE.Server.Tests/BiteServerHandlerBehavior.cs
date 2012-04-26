using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using BITE.Server.Plugins.Interfaces;
using BITE.Server.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;
using System.Web.Routing;

namespace BITE.Server.Tests
{
    [TestClass]
    public class BiteServerHandlerBehavior
    {
        private Mock<IAdapterFactory> _adapterFactory;
        private Mock<IMethodInvoker> _methodInvokerFactory;
        private BiteRequestHandler _handler;

        [TestInitialize]
        public void Init()
        {
            _adapterFactory = new Mock<IAdapterFactory>();
            _methodInvokerFactory = new Mock<IMethodInvoker>();
        }

        [TestMethod]
        public void ShouldLoadAdapterAndInvokeMethod()
        {
            var adapter = new FakeAdapter();
            var request = CreateRequestContext("bugs", "GET");
            _handler = new BiteRequestHandler(request, _adapterFactory.Object, _methodInvokerFactory.Object);

            _adapterFactory.Setup(f => f.CreateAdapter("bugs"))
               .Returns(adapter).Verifiable();

            _methodInvokerFactory.Setup(mi => mi.InvokeAdapterMethod(request, adapter))
               .Returns(GenericMethodInvocation).Verifiable();

            _handler.ProcessRequest(request.HttpContext);

            _adapterFactory.Verify();
            _methodInvokerFactory.Verify();
        }

        public Object GenericMethodInvocation()
        {
            return new object();
        }

        private static RequestContext CreateRequestContext(String adapterName, String method)
        {
            var stubHttpContext = new StubContext();
            var routeData = new RouteData();
            routeData.Values.Add("adapter", adapterName);

            var requestContext = new RequestContext(stubHttpContext, routeData);
                        
            var request = new StubRequest();

            request.SetRawUrl(adapterName);
            request.SetPath(adapterName);
            request.SetHttpMethod(method);

            stubHttpContext.SetRequest(request);
            stubHttpContext.SetResponse(new StubResponse());

            return requestContext;
        }
    }
}