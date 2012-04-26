using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using BITE.Server.Plugins.Attributes;
using BITE.Server.Plugins.Exceptions;
using BITE.Server.Plugins.Extensions;
using BITE.Server.Plugins.Interfaces;
using System.Text.RegularExpressions;

namespace BITE.Server.Plugins {
   public class DefaultAdapterFactory : IAdapterFactory {
       private Regex AdapterMatcher {get; set;} 

      private IDictionary<string, Type> AdapterMappings { get; set; } 

      public DefaultAdapterFactory() {
         AdapterMatcher = new Regex(Constants.RegularExpressions.AdapterTypeName, RegexOptions.Singleline | RegexOptions.Compiled);
         AdapterMappings = DiscoverBiteAdapters();
      }

      public object CreateAdapter(String adapterName) {
         if (AdapterMappings.ContainsKey(adapterName)) {
            return DependencyResolver.Default.GetService(AdapterMappings[adapterName]);
         }

         throw new AdapterNotFoundException(adapterName);
      }

      private IDictionary<String, Type> DiscoverBiteAdapters() {
         var mappingReturn = new Dictionary<String, Type>(StringComparer.OrdinalIgnoreCase);

         try {
            foreach (var mapping in GetAllMappings()) {
               if (!mappingReturn.ContainsKey(mapping.Key)) {
                  mappingReturn.Add(mapping.Key, mapping.Value);
               } else {
                  Trace.WriteLine("A duplicate AdapterName: {0} was found and will be ignored! ", mapping.Key);
               }
            }
         } catch (ReflectionTypeLoadException ex) {
            LogReflectionLoadException(ex);
         }

         return mappingReturn;
      }

      private IEnumerable<KeyValuePair<String, Type>> GetAllMappings() {
         var loggingDefinitionTypes = GetAllAdapterTypes();

         return from t in loggingDefinitionTypes
                let adapterName = GetAdapterName(t)
                where !String.IsNullOrWhiteSpace(adapterName)
                select new KeyValuePair<String, Type>(adapterName, t);
      }

      private String GetAdapterName(Type t)
      {
          String adapterName = null;
          var adapterAtt = t.GetCustomAttribute<BiteAdapterAttribute>();
          
          if (adapterAtt != null)
          {
              adapterName = adapterAtt.AdapterName;
          }
          else if(AdapterMatcher.IsMatch(t.Name))
          {
              var match = AdapterMatcher.Match(t.Name);
              adapterName = match.Groups["AdapterName"].Value;
          }

          return adapterName;
      }

      private IEnumerable<Type> GetAllAdapterTypes(){
         var assemblies = AppDomain.CurrentDomain.GetAssemblies();

         foreach (var assembly in assemblies) {
            var assemblyTypes = new List<Type>();

            try {
               assemblyTypes = assembly.GetTypes().ToList();
            } catch (ReflectionTypeLoadException ex) {
               LogReflectionLoadException(ex);
            }

            foreach (var type in assemblyTypes.Where(TypeIsAdapter)) {
               yield return type;
            }
         }
      }

      private bool TypeIsAdapter(Type type)
      {
          return type.IsPublic && type.IsClass && !type.Namespace.StartsWith("System") &&
                 (type.HasAttribute<BiteAdapterAttribute>() || 
                 AdapterMatcher.IsMatch(type.Name));
      }

      private void LogReflectionLoadException(ReflectionTypeLoadException ex) {
         var sbMsg = new StringBuilder(ex.ToString());
         sbMsg.AppendLine();
         sbMsg.AppendLine("------- Loader Exceptions --------");
         sbMsg.AppendLine();

         if (ex.LoaderExceptions != null) {
            foreach (var loaderException in ex.LoaderExceptions) {
               sbMsg.AppendLine(loaderException.ToString());
               sbMsg.AppendLine();
            }
         }

         Trace.WriteLine(sbMsg.ToString());
      }
   }
}