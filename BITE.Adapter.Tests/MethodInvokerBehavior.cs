using System;
using System.Web;
using BITE.Server.Plugins.Tests.FakeAdapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BITE.Server.Tests.Mocks;
using System.Web.Routing;
using BITE.Server.Plugins.Interfaces;
using Moq;
using System.Linq.Expressions;

namespace BITE.Server.Plugins.Tests
{
    [TestClass]
    public class MethodInvokerBehavior
    {
        private IMethodInvoker _methodInvoker;

        private Mock<IRequestFormatter> _formatter;

        [TestInitialize]
        public void Init()
        {
            _formatter = new Mock<IRequestFormatter>();
            _methodInvoker = new DefaultMethodInvoker(_formatter.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowExceptionIfNullFormatterPassedIn()
        {
            _methodInvoker = new DefaultMethodInvoker(null);
        }

        [TestMethod]
        public void ShouldInferMethodNameFromPath()
        {
            var request = CreateRequestContext("NamingConvention", "name_with_underscores");
            var adapter = new NamingConventionAdapter();

            _methodInvoker.InvokeAdapterMethod(request, adapter);

            Assert.IsTrue(adapter.WasMethodCalled(a => a.name_with_underscores()));
        }

        [TestMethod]
        public void ShouldCallMethodWithCorrectParameters()
        {
            var request = CreateRequestContext("NamingConvention", "name_with_underscores", query: "id=123&provider=fake&project=myProj");
            var adapter = new NamingConventionAdapter();

            _methodInvoker.InvokeAdapterMethod(request, adapter);

            Assert.IsTrue(adapter.WasMethodCalled(
                a => a.name_with_underscores(123, "fake", "myProj")));
        }

        [TestMethod]
        public void ShouldCallCorrectMethodOverloadBasedOnHttpVerb()
        {
            var request = CreateRequestContext("NamingConvention", "name_with_underscores", "POST");
            var adapter = new NamingConventionAdapter();

            _methodInvoker.InvokeAdapterMethod(request, adapter);

            Assert.IsTrue(adapter.WasMethodCalled(a => a.name_with_underscores(new { })));
        }

        [TestMethod]
        public void ShouldAssignIdIfPresentInParameters()
        {
           var request = CreateRequestContext("NamingConvention", "method_requires_id", "POST", id: "34");
            var adapter = new NamingConventionAdapter();

            _methodInvoker.InvokeAdapterMethod(request, adapter);

            Assert.IsTrue(adapter.WasMethodCalled(a => a.method_requires_id(34, new {})));
        }

        [TestMethod]
        public void ShouldCallMethodBasedOnAttribute()
        {
            var request = CreateRequestContext("Fake", "FakeMethodToCall");
            var adapter = new AttributeBasedAdapter();

            _methodInvoker.InvokeAdapterMethod(request, adapter);

            Assert.IsTrue(adapter.WasMethodCalled(a => a.AttributeDecoratedName()));
        }

        [TestMethod]
        public void ShouldCallMethodWithCorrectParametersBasedOnAttribute()
        {
            var request = CreateRequestContext("Fake", "FakeMethodToCall", query: "id=123&provider=fake&project=myProj");
            var adapter = new AttributeBasedAdapter();

            _methodInvoker.InvokeAdapterMethod(request, adapter);

            Assert.IsTrue(adapter.WasMethodCalled(
                a => a.AttributeDecoratedName(123, "fake", "myProj")));
        }

        [TestMethod]
        public void ShouldCallCorrectMethodWhenNoExplicitMethodNameIsGiven()
        {
            ConfirmMethodIsCalledForHttpVerbByNamingConvention("get", "", a => a.GetNamingConvention());
            ConfirmMethodIsCalledForHttpVerbByNamingConvention("post", "", a => a.PostNamingConvention());
            ConfirmMethodIsCalledForHttpVerbByNamingConvention("put", "", a => a.PutNamingConvention());
            ConfirmMethodIsCalledForHttpVerbByNamingConvention("patch", "", a => a.PatchNamingConvention());
            ConfirmMethodIsCalledForHttpVerbByNamingConvention("delete", "", a => a.DeleteNamingConvention());
        }

        [TestMethod]
        public void ShouldCallCorrectMethodMarkedWithCorrectHttpVerbByNamingConvention()
        {
            ConfirmMethodIsCalledForHttpVerbByNamingConvention("get", "MethodByHttpVerb", a => a.GetMethodByHttpVerb());
            ConfirmMethodIsCalledForHttpVerbByNamingConvention("post", "MethodByHttpVerb", a => a.PostMethodByHttpVerb());
            ConfirmMethodIsCalledForHttpVerbByNamingConvention("put", "MethodByHttpVerb", a => a.PutMethodByHttpVerb());
            ConfirmMethodIsCalledForHttpVerbByNamingConvention("patch", "MethodByHttpVerb", a => a.PatchMethodByHttpVerb());
            ConfirmMethodIsCalledForHttpVerbByNamingConvention("delete", "MethodByHttpVerb", a => a.DeleteMethodByHttpVerb());
        }

        [TestMethod]
        public void ShouldOnlyConsiderFirstPartOfMethodNameForHttpVerb()
        {
            ConfirmMethodIsCalledForHttpVerbByNamingConvention("get", "GetMethodByHttpVerb", a => a.GetGetMethodByHttpVerb());
            ConfirmMethodIsCalledForHttpVerbByNamingConvention("post", "PostMethodByHttpVerb", a => a.PostPostMethodByHttpVerb());
            ConfirmMethodIsCalledForHttpVerbByNamingConvention("put", "PutMethodByHttpVerb", a => a.PutPutMethodByHttpVerb());
            ConfirmMethodIsCalledForHttpVerbByNamingConvention("patch", "PatchMethodByHttpVerb", a => a.PatchPatchMethodByHttpVerb());
            ConfirmMethodIsCalledForHttpVerbByNamingConvention("delete", "DeleteMethodByHttpVerb", a => a.DeleteDeleteMethodByHttpVerb());
        }

        [TestMethod]
        public void ShouldCallCorrectMethodMarkedWithCorrectHttpVerb()
        {
            ConfirmMethodIsCalledForHttpVerb("post", a => a.AttributeDecoratedName(new { }));
            ConfirmMethodIsCalledForHttpVerb("put", a => a.AttributeDecoratedName_Put(new { }));
            ConfirmMethodIsCalledForHttpVerb("patch", a => a.AttributeDecoratedName_Patch(new { }));
            ConfirmMethodIsCalledForHttpVerb("delete", a => a.AttributeDecoratedName_Delete(new { }));
        }

        private void ConfirmMethodIsCalledForHttpVerbByNamingConvention(String httpVerb, String methodName, Expression<Action<NamingConventionAdapter>> expectedMethod)
        {
            var request = CreateRequestContext("NamingConvention", methodName, httpVerb);
            var adapter = new NamingConventionAdapter();

            _methodInvoker.InvokeAdapterMethod(request, adapter);

            Assert.IsTrue(adapter.WasMethodCalled(expectedMethod), "Expected method {0}{1} to be called", httpVerb, methodName);
        }

        private void ConfirmMethodIsCalledForHttpVerb(String httpVerb, Expression<Action<AttributeBasedAdapter>> expectedMethod)
        {
            var request = CreateRequestContext("Fake", "FakeMethodToCall", httpVerb);
            var adapter = new AttributeBasedAdapter();

            _methodInvoker.InvokeAdapterMethod(request, adapter);

            Assert.IsTrue(adapter.WasMethodCalled(expectedMethod));
        }

        private static RequestContext CreateRequestContext(String adapterName, String method, String httpVerb = "GET", String query = null, String id = null)
        {
            var stubHttpContext = new StubContext();
            var routeData = new RouteData();
            routeData.Values.Add("adapter", adapterName);
            routeData.Values.Add("method", method);

            if (id != null)
            {
                routeData.Values.Add("id", id);
            }

            var requestContext = new RequestContext(stubHttpContext, routeData);

            var request = new StubRequest();

            request.SetRawUrl(adapterName);
            request.SetPath(adapterName);
            request.SetHttpMethod(httpVerb);
            request.SetQueryString(query);

            stubHttpContext.SetRequest(request);
            stubHttpContext.SetResponse(new StubResponse());

            return requestContext;
        }
    }
}