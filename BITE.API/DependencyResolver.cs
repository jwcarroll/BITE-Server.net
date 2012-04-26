using System;
using BITE.Server.Plugins.Interfaces;

namespace BITE.Server.Plugins {
   public static class DependencyResolver {
      public static IDependencyResolver Default { get; private set; }

      public static void SetResolver(IDependencyResolver resolver) {
         if(resolver == null) throw new ArgumentNullException("resolver");

         Default = resolver;
      }

      public static T GetService<T>(this IDependencyResolver resolver) {
         return (T) resolver.GetService(typeof (T));
      }
   }
}
