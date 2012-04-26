using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.IO;
using System.Web.Routing;
using BITE.Server.Plugins.Interfaces;

namespace BITE.Server {
   public class BiteRequest {
      public BiteRequest(HttpRequestBase request) {
         RequestUrl = request.RawUrl;
         Method = request.HttpMethod;
         AdapterName = "[none]";
         AdapterType = GetAdapterType(null);
      }

      public BiteRequest(InvokeAdapterMethodEventArgs eventArgs) {
         var context = eventArgs.Context;

         RequestUrl = context.HttpContext.Request.RawUrl;
         Method = context.HttpContext.Request.HttpMethod;
         AdapterName = context.RouteData.GetRequiredString("adapter");
         MethodInvoked = eventArgs.Method.Method.Name;
         AdapterType = eventArgs.AdapterType.FullName;
         ReturnData = GetJson(eventArgs.ReturnVal);
         RequestArgs = GetJson(eventArgs.RequestArgs);
         PassedArgs = GetJson(eventArgs.Args);
      }

      private static String GetAdapterType(object adapter) {
         return adapter == null ? "[null]" : adapter.GetType().FullName;
      }

      private static String GetJson(Object obj) {
         return obj == null ? null : Json.Encode(obj);
      }

      public String Method { get; set; }
      public String RequestUrl { get; set; }
      public String AdapterName { get; set; }
      public String MethodInvoked { get; set; }
      public String AdapterType { get; set; }
      public String ReturnData { get; set; }
      public String RequestArgs { get; set; }
      public String PassedArgs { get; set; }
   }
}
