using System.Web;

namespace BITE.Server.Tests.Mocks {
   public class StubContext: HttpContextBase {

      private HttpResponseBase _response;
      public override HttpResponseBase Response {
         get {
            return _response ?? (_response = new StubResponse());
         }
      }

      public void SetResponse(HttpResponseBase response) {
         _response = response;
      }

      private HttpRequestBase _request;
      public override HttpRequestBase Request {
         get {
            return _request ?? (_request = new StubRequest());
         }
      }

      public void SetRequest(HttpRequestBase request) {
         _request = request;
      }
   }
}
