using System;
using ExampleAdapters.Models;

namespace ExampleAdapters.Adapters {
   public class BugsAdapter {
      public object PostUrls(Object bugRequest) {
         return new BugMappings();
      }
   }
}