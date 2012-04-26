using System;

namespace BITE.Server.Plugins.Attributes {
   public class BiteAdapterAttribute : Attribute {
      public string AdapterName { get; set; }
   }
}
