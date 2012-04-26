using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace BITE.Server.Plugins.Interfaces {
   public class InvokeAdapterMethodEventArgs: EventArgs {
      private readonly RequestContext _context;
      private readonly Type _adapterType;
      private readonly AdapterMethod _method;
      private readonly object[] _requestArgs;
      private readonly object[] _passedArgs;
      private readonly object _returnVal;

      public InvokeAdapterMethodEventArgs(RequestContext context, Type adapterType, AdapterMethod method, object[] requestArgs, object[] passedArgs, object returnVal) {
         _context = context;
         _adapterType = adapterType;
         _method = method;
         _requestArgs = requestArgs;
         _passedArgs = passedArgs;
         _returnVal = returnVal;
      }

      public object ReturnVal {
         get {
            return _returnVal;
         }
      }

      public object[] RequestArgs {
         get {
            return _requestArgs;
         }
      }

      public object[] Args {
         get {
            return _passedArgs;
         }
      }

      public AdapterMethod Method {
         get {
            return _method;
         }
      }

      public Type AdapterType {
         get {
            return _adapterType;
         }
      }

      public RequestContext Context {
         get {
            return _context;
         }
      }
   }
}
