using System.Web.Mvc;
using BITE.ServerInspector.DependencyResolution;
using StructureMap;

[assembly: WebActivator.PreApplicationStartMethod(typeof(BITE.ServerInspector.App_Start.StructuremapMvc), "Start")]

namespace BITE.ServerInspector.App_Start {
    public static class StructuremapMvc {
        public static void Start() {
            var container = (IContainer) IoC.Initialize();
            DependencyResolver.SetResolver(new MvcStructureMapResolver(container));
        }
    }
}