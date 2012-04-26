using System;
using System.Collections.Generic;

namespace ExampleAdapters.Models {
   public class BugMappings {
      public BugMappings() {
         mappings = new List<BugMapping>();
      }

      public String kind {
         get {
            return "bugs#url-bug-map";
         }
      }

      public List<BugMapping> mappings { get; private set; }
   }
}