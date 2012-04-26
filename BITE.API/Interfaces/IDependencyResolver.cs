using System;

namespace BITE.Server.Plugins.Interfaces {
   public interface IDependencyResolver {
      object GetService(Type serviceType);
   }
}