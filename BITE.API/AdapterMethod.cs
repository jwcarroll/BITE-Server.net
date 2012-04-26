using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using BITE.Server.Plugins.Extensions;
using BITE.Server.Plugins.Attributes;
using System.Text.RegularExpressions;

namespace BITE.Server.Plugins
{
   public sealed class AdapterMethod
    {
        private const String NamedGroupHttpMethod = "HttpMethod";
        private readonly Regex _httpMethodNamePattern = new Regex("^(?<HttpMethod>get|post|put|patch|delete)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        internal AdapterMethod(MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException("method");

            Method = method;

            InitProperties();
        }

        private void InitProperties()
        {
            var att = Method.GetCustomAttribute<BiteMethodAttribute>();

            if (att != null)
                InitPropertiesFromAttribute(att);
            else
                InitPropertiesFromMethod(Method);
        }

        private void InitPropertiesFromMethod(MethodInfo method)
        {
            Name = _httpMethodNamePattern.Replace(method.Name, String.Empty);
            HttpMethod = InferHttpMethodFromName(method) ?? InferHttpMethodFromParams(method.GetParameters());
        }

        private void InitPropertiesFromAttribute(BiteMethodAttribute att)
        {
            Name = att.Name;
            HttpMethod = att.HttpMethod;
        }

        private string InferHttpMethodFromName(MethodInfo method)
        {
            if (method == null) return null;

            var match = _httpMethodNamePattern.Match(method.Name);

            if(!match.Success) return null;

            return match.Groups[NamedGroupHttpMethod].Value.ToUpperInvariant();
        }

        private string InferHttpMethodFromParams(IEnumerable<ParameterInfo> parameterInfo)
        {
            if (parameterInfo != null && IsFinalParamOfType<Object>(parameterInfo))
                return "POST";

            return "GET";
        }

       private static bool IsFinalParamOfType<T>(IEnumerable<ParameterInfo> parameterInfo) {
          return parameterInfo != null && 
             parameterInfo.Any() && 
             parameterInfo.OrderBy(pi => pi.Position).Last().ParameterType == typeof(T);
       }

       public String Name { get; set; }

        public String HttpMethod { get; set; }

        public MethodInfo Method { get; set; }
    }
}
