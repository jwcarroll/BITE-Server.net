using BITE.Server.Plugins.Exceptions;
using BITE.Server.Plugins.Interfaces;
using BITE.Server.Plugins.Tests.FakeAdapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BITE.Server.Plugins.Tests {
   [TestClass]
   public class AdapterFactoryBehavior {
      private Mock<IDependencyResolver> _dependencyResolver;
      private IAdapterFactory _adapterFactory;

      [TestInitialize]
      public void Init() {
         _dependencyResolver = new Mock<IDependencyResolver>();
         DependencyResolver.SetResolver(_dependencyResolver.Object);
         _adapterFactory = new DefaultAdapterFactory();
      }

      [TestMethod]
      public void ShouldCreateAdapterBasedOnAttribute() {
         _dependencyResolver.Setup(svc => svc.GetService(typeof (AttributeBasedAdapter)))
            .Returns(new AttributeBasedAdapter());

         var adapter = _adapterFactory.CreateAdapter("Fake");

         Assert.IsTrue(adapter is AttributeBasedAdapter);
      }

      [TestMethod]
      public void ShouldResolveAdapterBasedOnNamingConvention() {
         _dependencyResolver.Setup(svc => svc.GetService(typeof (NamingConventionAdapter)))
            .Returns(new NamingConventionAdapter());

         var adapter = _adapterFactory.CreateAdapter("NamingConvention");

         Assert.IsTrue(adapter is NamingConventionAdapter);
      }

      [TestMethod]
      [ExpectedException(typeof (AdapterNotFoundException))]
      public void ShouldNotLoadInvalidNamingConventionAdapters() {
         _adapterFactory.CreateAdapter("Invalid");
      }

      [TestMethod]
      [ExpectedException(typeof (AdapterNotFoundException))]
      public void ShouldThrowExceptionIfAdapterNotFound() {
         _adapterFactory.CreateAdapter("bogusPath");
      }
   }
}
