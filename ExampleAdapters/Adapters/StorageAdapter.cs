using BITE.Server.Plugins.Attributes;

namespace ExampleAdapters.Adapters {
   public class StorageAdapter {
      public object getprojectnames() {
         return new string [] {
            "Mapping Application",
            "Survey",
            "Internal Project X"
         };
      }

      [BiteMethod("addtest","POST")]
      public object addtest()
      {
          // TODO: This is NOT correct.  The client expects an id parameter back, but it is not expecting it to be JSON formatted (it should be Content-Type text/html)
          // An example correct response would be:
          //    id=b99b5cf3-c59b-485f-90ce-9925a34eff46

          return new {
                         id = "b99b5cf3-c59b-485f-90ce-9925a34eff46"
                     };
      }
   }
}
