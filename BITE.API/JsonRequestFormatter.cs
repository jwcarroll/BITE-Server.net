using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using BITE.Server.Plugins.Interfaces;
using System.IO;

namespace BITE.Server.Plugins {
   public class JsonRequestFormatter: IRequestFormatter {
      public object FormatRequest(HttpRequestBase request) {
         if (request == null)
            return null;

         var reader = new StreamReader(request.InputStream, request.ContentEncoding);

         return System.Web.Helpers.Json.Decode(reader.ReadToEnd());
      }
   }
}
