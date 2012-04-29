using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using BITE.Server.Plugins.Interfaces;
using BITE.Server.Plugins.Extensions;
using BITE.Server.Plugins.Attributes;
using System.Reflection;
using System.Web.Routing;
using System.IO;
using BITE.Server.Plugins.Exceptions;
using System.Web.Helpers;
using Microsoft.CSharp.RuntimeBinder;
using Binder = System.Reflection.Binder;

namespace BITE.Server.Plugins
{
    public class DefaultMethodInvoker : IMethodInvoker
    {
        private IRequestFormatter Formatter { get; set; }

        public DefaultMethodInvoker(IRequestFormatter formatter)
        {
            if (formatter == null) throw new ArgumentNullException("formatter");

            Formatter = formatter;
        }

        public object InvokeAdapterMethod(RequestContext request, Object adapter)
        {
            var methodName = GetMethodName(request);
            var args = CreateArgumentList(request);
            var httpMethod = request.HttpContext.Request.HttpMethod;

            var method = GetMethodInfo(methodName, httpMethod, args, adapter.GetType());
           var convertedArgs = ConvertArgs(args, method.Method.GetParameters());
           var returnVal = method.Method.Invoke(adapter, convertedArgs);

           OnMethodInvoked(request, adapter.GetType(), method, args, convertedArgs, returnVal);

           return returnVal;
        }

       public event EventHandler<InvokeAdapterMethodEventArgs> MethodInvoked;

       private void OnMethodInvoked(RequestContext context, Type adapterType, AdapterMethod method, object[] requestArgs, object[] passedArgs, object returnVal) {
          var handler = MethodInvoked;
          if (handler != null) {
             handler(this, new InvokeAdapterMethodEventArgs(context, adapterType, method, requestArgs, passedArgs, returnVal));
          }
       }

       private String GetMethodName(RequestContext request)
        {
            try
            {
                return request.RouteData.GetRequiredString("method");
            }
            catch
            {
                return request.RouteData.GetRequiredString("adapter");
            }
        }

        private object[] ConvertArgs(object[] args, ParameterInfo[] parameterInfo)
        {
            if (args == null && parameterInfo == null) return null;
            if (args.Length == 0 && parameterInfo.Length == 0) return null;

            var argsList = new List<Object>(args);

            // Check for special case of a single JSON object
            if (argsList.Count == 1 && argsList[0] is DynamicJsonObject)
            {
                argsList = new List<object>(ConvertDynamicJsonObjectToArgumentArray((DynamicJsonObject)argsList[0]));
            }

            var convertedArgs = new List<Object>();

            foreach (var paramInfo in parameterInfo)
            {
                var arg = PopArgumentFromList(paramInfo, argsList);

                ValidateArgument(paramInfo, arg);

                convertedArgs.Add(ConvertArg(arg, paramInfo));
            }

            return convertedArgs.ToArray();
        }

        private object[] ConvertDynamicJsonObjectToArgumentArray(DynamicJsonObject jsonObject)
        {
            var convertedProperties = new List<object>();

            var dynamicMemberNames = jsonObject.GetDynamicMemberNames();
            foreach (var dynamicMemberName in dynamicMemberNames)
            {
                object tempObject = GetDynamicMember(jsonObject, dynamicMemberName);
                convertedProperties.Add(new QueryStringParam(dynamicMemberName, tempObject.ToString()));
            }

            return convertedProperties.ToArray();
        }

        private static object GetDynamicMember(object obj, string memberName)
        {
            var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, memberName, obj.GetType(),
                new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
            return callsite.Target(callsite, obj);
        }

        private object ConvertArg(object arg, ParameterInfo paramInfo)
        {
            if (arg == null || arg.GetType() == paramInfo.ParameterType)
                return arg;

            return Convert.ChangeType(arg, paramInfo.ParameterType);
        }

        private void ValidateArgument(ParameterInfo paramInfo, object arg)
        {
            if (!IsParamOptional(paramInfo) && arg == null)
                throw new ArgumentException(String.Format("{0} is not an optional parameter but no value was provided"), paramInfo.Name);
        }

        private bool IsParamOptional(ParameterInfo paramInfo)
        {
            return paramInfo.ParameterType.IsClass;
        }

        private object PopArgumentFromList(ParameterInfo paramInfo, List<Object> argsList)
        {
            Object argToReturn = null;

            var findByName = argsList.Where(a => a is QueryStringParam).Cast<QueryStringParam>()
                .Where(a => String.Equals(a.Name, paramInfo.Name, StringComparison.OrdinalIgnoreCase))
                .Select(qsp => qsp.Value).FirstOrDefault();

            if (findByName != null){
                argToReturn = findByName;
            }
            else{
                argToReturn = argsList.Where(a => a != null)
                    .FirstOrDefault(a => a.GetType() == paramInfo.ParameterType);
            }

            argsList.Remove(argToReturn);

            return argToReturn;
        }

        private object[] CreateArgumentList(RequestContext request)
        {
            var argsList = new List<Object>();

            var id = ExtractId(request);

            if (id.HasValue)
                argsList.Add(id.Value);

            if (!IsGetRequest(request))
            {
                argsList.Add(Formatter.FormatRequest(request.HttpContext.Request));
            }
            else if (ContainsQueryString(request))
            {
                argsList.AddRange(ExtractQueryStringValues(request));
            }
            
            return argsList.ToArray();
        }

        private bool ContainsQueryString(RequestContext request)
        {
            return request.HttpContext.Request.QueryString.Count > 0;
        }

        private IEnumerable<Object> ExtractQueryStringValues(RequestContext request)
        {
            var query = request.HttpContext.Request.QueryString;

            for (var i = 0; i < query.Count; i++ )
            {
                yield return new QueryStringParam(query.Keys[i], query.Get(i));
            }
        }

        private Int32? ExtractId(RequestContext request)
        {
            Int32? id = null;

            if (request.RouteData.Values.ContainsKey("id"))
            {
                var strId = request.RouteData.Values["id"] as String;
                var tmpId = 0;

                if (Int32.TryParse(strId, out tmpId))
                {
                    id = tmpId;
                }
            }

            return id;
        }

        private bool IsGetRequest(RequestContext request)
        {
            return String.Equals(request.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase);
        }

        private AdapterMethod GetMethodInfo(String methodName, String httpMethod, Object[] args, Type adapterType)
        {
            var suitableMethods = (from m in adapterType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                                   let am = new AdapterMethod(m)
                                   let aInfo = new{
                                        adapterMethod = am,
                                        nameMatches = String.Equals(am.Name, methodName, StringComparison.OrdinalIgnoreCase),
                                        argsMatch = am.Method.GetParameters().Length == args.Length,
                                        httpMethodMatches = String.Equals(am.HttpMethod, httpMethod, StringComparison.OrdinalIgnoreCase)
                                   }
                                   orderby aInfo.nameMatches descending, aInfo.argsMatch descending
                                   where aInfo.httpMethodMatches
                                   select aInfo).ToList();
            
            if(suitableMethods.Count == 0)
                throw new MethodNotFoundException(methodName, args, adapterType);

            return suitableMethods[0].adapterMethod;
        }

        class QueryStringParam
        {
            public QueryStringParam(string name, string value)
            {
                Name = name;
                Value = value;
            }

            internal String Name { get; set; }
            internal String Value { get; set; }

            public override string ToString()
            {
                return String.Format("{{'{0}'}}:{{{1}}}", Name, Value);
            }
        }
    }
}
