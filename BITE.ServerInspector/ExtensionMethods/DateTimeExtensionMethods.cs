using System;

namespace BITE.ServerInspector.ExtensionMethods {
   public static class DateTimeExtensionMethods {
      // returns the number of milliseconds since Jan 1, 1970 (useful for converting C# dates to JS dates)
      public static double UnixTicks(this DateTime dt) {
         var d1 = new DateTime(1970, 1, 1);
         var ts = new TimeSpan(dt.Ticks - d1.Ticks);
         return ts.TotalMilliseconds;
      }
   }
}