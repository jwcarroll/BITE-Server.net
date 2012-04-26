using StructureMap;
using BITE.Server.Plugins.Interfaces;
using BITE.Server.Plugins;

namespace BITE.ServerInspector.DependencyResolution {
   public static class IoC {
      public static IContainer Initialize() {
         ObjectFactory.Initialize(x => {
            x.Scan(scan => {
               scan.TheCallingAssembly();
               scan.WithDefaultConventions();
            });

            x.For<IAdapterFactory>().Use<DefaultAdapterFactory>();
            x.For<IMethodInvoker>().Use<DefaultMethodInvoker>();
            x.For<IRequestFormatter>().Use<JsonRequestFormatter>();
         });
         return ObjectFactory.Container;
      }
   }
}