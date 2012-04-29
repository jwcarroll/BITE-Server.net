using System;
using ExampleAdapters.Models;

namespace ExampleAdapters.Adapters {
   public class BugsAdapter {
       public object Post(Object title)
       {
           return new {
                          kind = "bugs#id",
                          id = 1234
                      };
       }

      public object PostUrls(Object bugRequest) {
         return new BugMappings();
      }
   }
}