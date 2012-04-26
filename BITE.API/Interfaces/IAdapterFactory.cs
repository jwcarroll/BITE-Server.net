using System;

namespace BITE.Server.Plugins.Interfaces {
   public interface IAdapterFactory {
      Object CreateAdapter(String path);
   }
}