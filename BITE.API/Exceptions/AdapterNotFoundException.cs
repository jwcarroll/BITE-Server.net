using System;
using System.Runtime.Serialization;
using System.Text;

namespace BITE.Server.Plugins.Exceptions {
   [Serializable]
   public class AdapterNotFoundException : Exception {
      public String AdapterName { get; private set; }
      private const String AdapterNamePropertyName = "AdapterName";

      public AdapterNotFoundException(String path) {
         AdapterName = path;
      }

      public AdapterNotFoundException(String path, String message) : base(message) {
         AdapterName = path;
      }

      public AdapterNotFoundException(String path, String message, Exception innerException) : base(message, innerException) {
         AdapterName = path;
      }

      protected AdapterNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {
         if (info != null) {
            AdapterName = info.GetString(AdapterNamePropertyName);
         }
      }

      public override void GetObjectData(SerializationInfo info, StreamingContext context) {
         base.GetObjectData(info, context);

         info.AddValue(AdapterNamePropertyName, AdapterName);
      }

      public override string Message {
         get {
            var sb = new StringBuilder(base.Message);

            sb.AppendLine();
            sb.AppendFormat("Unable to find a type with the name: \"{0}Adapter\", or one decorated with [BiteAdapter(AdapterName=\"{0}\")]", AdapterName);
            
            return sb.ToString();
         }
      }
   }
}