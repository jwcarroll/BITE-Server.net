using System;
using System.Linq;
using System.Reflection;

namespace BITE.Server.Plugins.Extensions {
   public static class ReflectionExtensions {

      public static TAtt GetCustomAttribute<TAtt>(this Type type) where TAtt : Attribute {
         var attributes = type.GetCustomAttributes(typeof (TAtt), true);
         return (TAtt) attributes.FirstOrDefault();
      }

      public static TAtt GetCustomAttribute<TAtt>(this MemberInfo type) where TAtt : Attribute {
         var attributes = type.GetCustomAttributes(typeof (TAtt), true);
         return (TAtt) attributes.FirstOrDefault();
      }

      public static Boolean HasAttribute<TAttr>(this PropertyInfo propertyInfo) where TAttr : Attribute {
         return propertyInfo.GetCustomAttribute<TAttr>() != null;
      }

      public static Boolean HasAttribute<TAttr>(this Type type) where TAttr : Attribute {
         return type.GetCustomAttribute<TAttr>() != null;
      }

      public static Boolean Implements<T>(this Type typeToCheck) {
         return typeToCheck.Implements(typeof (T));
      }

      public static Boolean Implements(this Type typeToCheck, Type typeToCheckFor) {
         if (typeToCheck == null || typeToCheckFor == null) {
            return false;
         }
         if (typeToCheck == typeToCheckFor) {
            return true;
         }
         return typeToCheckFor.IsInterface ? typeToCheck.ImplementsInterface(typeToCheckFor) : typeToCheckFor.IsAssignableFrom(typeToCheck);
      }

      private static Boolean ImplementsInterface(this Type typeToCheck, Type typeToCheckFor) {
         if (typeToCheck == null || typeToCheckFor == null) {
            return false;
         }

         bool implementsInterface;

         if (typeToCheckFor.IsGenericType) {
            implementsInterface = typeToCheck.GetInterfaces().Any(x =>
                                                                  x.IsGenericType &&
                                                                  x.GetGenericTypeDefinition() == typeToCheckFor);
         }
         else {
            implementsInterface = typeToCheckFor.IsAssignableFrom(typeToCheck);
         }

         return implementsInterface;
      }
   }
}
