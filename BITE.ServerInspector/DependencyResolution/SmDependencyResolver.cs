using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using StructureMap;

namespace BITE.ServerInspector.DependencyResolution
{
    public class SmDependencyResolver {

        private readonly IContainer _container;

        public SmDependencyResolver(IContainer container) {
            _container = container;
        }

        public object GetService(Type serviceType) {
            if (serviceType == null) return null;

           return serviceType.IsAbstract || serviceType.IsInterface
                     ? _container.TryGetInstance(serviceType)
                     : _container.GetInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return _container.GetAllInstances(serviceType).Cast<object>();
        }
    }

   public class MvcStructureMapResolver: SmDependencyResolver, IDependencyResolver {
      public MvcStructureMapResolver(IContainer container) : base(container) {}
   }

   public class BiteStructureMapResolver: SmDependencyResolver, Server.Plugins.Interfaces.IDependencyResolver {
      public BiteStructureMapResolver(IContainer container) : base(container) {}
   }
}