using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace BITE.Server.Tests.Mocks {
   public class StubRequest: HttpRequestBase {

      public override string ContentType { get; set; }

      public String Data { get; set; }

      public override Stream InputStream {
         get {
            return ConvertToStream(Data ?? String.Empty);
         }
      }

      private string _rawUrl;
      public override string RawUrl {
         get {
            return _rawUrl;
         }
      }

      public void SetRawUrl(String rawUrl) {
         _rawUrl = rawUrl;
      }

      private string _path;
      public override string Path {
         get {
            return _path;
         }
      }

      public void SetPath(String path) {
         _path = path;
      }

      private string _httpMethod;
      public override string HttpMethod {
         get {
            return _httpMethod;
         }
      }

      public void SetHttpMethod(String method) {
         _httpMethod = method;
      }

      private NameValueCollection _queryString = new NameValueCollection();
      public override NameValueCollection QueryString
      {
          get
          {
              return _queryString;
          }
      }

      public void SetQueryString(String query)
      {
          if (String.IsNullOrWhiteSpace(query)) return;

          _queryString.Clear();

          var nvp = from kvpRaw in query.Split('&')
                    let kvp = kvpRaw.Split('=')
                    select new { name = kvp[0], value = kvp[1] };

          foreach (var pair in nvp)
          {
              _queryString.Add(pair.name, pair.value);
          }
      }

      private Stream ConvertToStream(String data) {
         var bytes = Encoding.UTF8.GetBytes(data);
         return new MemoryStream(bytes);
      }
   }
}
