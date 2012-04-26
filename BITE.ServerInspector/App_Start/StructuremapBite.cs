using BITE.Server;
using BITE.Server.Plugins;
using BITE.ServerInspector.DependencyResolution;
using StructureMap;

[assembly: WebActivator.PreApplicationStartMethod(typeof(BITE.ServerInspector.App_Start.StructuremapBite), "Start")]

namespace BITE.ServerInspector.App_Start {
    public static class StructuremapBite {
        public static void Start() {
            var container = (IContainer) IoC.Initialize();
            DependencyResolver.SetResolver(new BiteStructureMapResolver(container));
        }
    }
}