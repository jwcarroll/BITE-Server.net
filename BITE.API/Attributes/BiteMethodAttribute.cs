using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BITE.Server.Plugins.Attributes
{
    public class BiteMethodAttribute : Attribute
    {
        public String Name { get; set; }
        public String HttpMethod { get; set; }

        public BiteMethodAttribute(String name) : this(name, "GET") { }

        public BiteMethodAttribute(String name, String httpMethod)
        {
            Name = name;
            HttpMethod = Normalize(httpMethod);
        }

        private string Normalize(string httpMethod)
        {
            if (String.IsNullOrWhiteSpace(httpMethod))
                return "GET";

            return httpMethod.ToUpperInvariant();
        }
    }
}
