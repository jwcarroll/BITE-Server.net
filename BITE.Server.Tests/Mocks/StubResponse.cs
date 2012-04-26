using System.Web;

namespace BITE.Server.Tests.Mocks {
   public class StubResponse : HttpResponseBase {
      public override string ContentType { get; set; }
      
      public override void Write(string s) {
         //No Op
      }
   }
}