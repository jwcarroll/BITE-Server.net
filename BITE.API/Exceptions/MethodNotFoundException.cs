using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BITE.Server.Plugins.Exceptions
{
	[Serializable]
	public class MethodNotFoundException : Exception
	{
		private const String AdapterTypeKey = "AdapterType";
		public Type AdapterType { get; private set; }

		private const String ArgumentsKey = "Arguments";
		public List<Object> Arguments { get; private set; }

        private const String MethodNameKey = "MethodName";
		public String MethodName { get; private set; }

		public MethodNotFoundException(String methodName, Object[] args, Type adapterType)
		{
			MethodName = methodName;
			Arguments = new List<Object>(args);
			AdapterType = adapterType;
		}
		
		protected MethodNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) 
		{
			if(info != null){
				MethodName = info.GetString(MethodNameKey);
				Arguments = (List<Object>)info.GetValue(ArgumentsKey, typeof(List<Object>));
				AdapterType = (Type)info.GetValue(AdapterTypeKey, typeof(Type));
			}
		}

		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue(MethodNameKey, MethodName);
			info.AddValue(ArgumentsKey, Arguments);
			info.AddValue(AdapterTypeKey, AdapterType);
		}

		public override string Message
		{
			get 
			{ 
				var msg = new StringBuilder(base.Message);

				msg.AppendLine();
				msg.AppendFormat("Unable to locate a suitable method for Adapter: {0}", AdapterType.Name);
				msg.AppendLine();
				msg.AppendFormat("Method Signature: {0}({1})", MethodName, FormatArguments(Arguments));

                return msg.ToString();
			}
		}

		private String FormatArguments(List<object> args)
		{
			if(args == null || args.Count == 0)
				return String.Empty;

			return String.Join(",", args);
		}
	}
}
