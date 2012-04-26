using System;
using System.Collections.Generic;
using BITE.Server.Plugins.Attributes;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BITE.Server.Plugins.Tests.FakeAdapters
{
    public static class TestAdapterHelpers
    {
        public static Boolean WasMethodCalled<T>(this T target, Expression<Action<T>> expression) where T : TestAdapter
        {
            return target.WasMethodCalled(
                expression.ExtractMethodName(),
                expression.ExtractCallingParameters());
        }

        public static String GetMethodSignature(this MethodBase methodBase)
        {
            var paramTypes = methodBase.GetParameters().Select(pi => pi.ParameterType.Name);
            var paramString = String.Join(",", paramTypes);

            return String.Format("{0}({1})", methodBase.Name, paramString);
        }

        private static String ExtractMethodName<T>(this Expression<Action<T>> expression)
        {
            var methodCall = expression.Body as MethodCallExpression;

            if (methodCall != null)
            {
                return methodCall.Method.GetMethodSignature();
            }

            throw new ArgumentException("Unable to extract method name from expression", "expression");
        }

        private static Object[] ExtractCallingParameters<T>(this Expression<Action<T>> expression)
        {
            var methodCall = expression.Body as MethodCallExpression;

            if (methodCall == null) throw new ArgumentException("Unable to extract calling parameters from expression", "expression");

            return (from a in methodCall.Arguments
                    select GetValueFromArgumentExpression(a)).ToArray();
        }

        private static object GetValueFromArgumentExpression(Expression a)
        {
            var constExpression = a as ConstantExpression;

            if (constExpression == null) return null;

            return constExpression.Value;
        }
    }

    public abstract class TestAdapter
    {
        private readonly Dictionary<String, Object[]> _functionCalls = new Dictionary<String, Object[]>();

        protected void LogFunctionCall(params Object[] callingParameters)
        {
            var stack = new StackTrace(0);
            var caller = stack.GetFrame(1).GetMethod();
            var name = caller.GetMethodSignature();

            Trace.Write(String.Format("{0} - [{1}]", name, FormatParams(callingParameters)));

            _functionCalls[name] = callingParameters;
        }

        private String FormatParams(object[] callingParameters)
        {
            if (callingParameters == null || callingParameters.Length == 0)
                return String.Empty;

            return String.Join(",", callingParameters);
        }

        public Boolean WasMethodCalled(String methodName, params Object[] expectedParameters)
        {
            if (_functionCalls.ContainsKey(methodName))
            {
                return ParamsMatch(_functionCalls[methodName], expectedParameters);
            }

            return false;
        }

        private Boolean ParamsMatch(object[] actualParams, object[] expectedParams)
        {
            if (actualParams == null && expectedParams == null) return true;
            if (actualParams == null || expectedParams == null) return false;

            return actualParams.SequenceEqual(expectedParams);
        }
    }

    public class NamingConventionAdapter : TestAdapter
    {
        public object name_with_underscores()
        {
            LogFunctionCall();

            return new { };
        }

        public object name_with_underscores(dynamic input)
        {
            var @params = new[] { (Object)input };
            LogFunctionCall(@params);

            return new { };
        }
        
        public object method_requires_id(Int32 id, dynamic input)
        {
            var @params = new[] { id, (Object)input };
            LogFunctionCall(@params);

            return new { };
        }

        public object name_with_underscores(Int32 id, String provider, String project)
        {
            LogFunctionCall(id, provider, project);

            return new { };
        }

        public object GetMethodByHttpVerb()
        {
            LogFunctionCall();

            return new { };
        }

        public object PostMethodByHttpVerb()
        {
            LogFunctionCall();

            return new { };
        }

        public object PutMethodByHttpVerb()
        {
            LogFunctionCall();

            return new { };
        }

        public object PatchMethodByHttpVerb()
        {
            LogFunctionCall();

            return new { };
        }

        public object DeleteMethodByHttpVerb()
        {
            LogFunctionCall();

            return new { };
        }

        public object GetGetMethodByHttpVerb()
        {
            LogFunctionCall();

            return new { };
        }

        public object PostPostMethodByHttpVerb()
        {
            LogFunctionCall();

            return new { };
        }

        public object PutPutMethodByHttpVerb()
        {
            LogFunctionCall();

            return new { };
        }

        public object PatchPatchMethodByHttpVerb()
        {
            LogFunctionCall();

            return new { };
        }

        public object DeleteDeleteMethodByHttpVerb()
        {
            LogFunctionCall();

            return new { };
        }

        public object GetNamingConvention()
        {
            LogFunctionCall();

            return new { };
        }

        public object PostNamingConvention()
        {
            LogFunctionCall();

            return new { };
        }

        public object PutNamingConvention()
        {
            LogFunctionCall();

            return new { };
        }

        public object PatchNamingConvention()
        {
            LogFunctionCall();

            return new { };
        }

        public object DeleteNamingConvention()
        {
            LogFunctionCall();

            return new { };
        }
    }

    [BiteAdapter(AdapterName = "Fake")]
    public class AttributeBasedAdapter : TestAdapter
    {
        [BiteMethod("FakeMethodToCall")]
        public object AttributeDecoratedName()
        {
            LogFunctionCall();

            return new { };
        }

        [BiteMethod("FakeMethodToCall", HttpMethod="POST")]
        public object AttributeDecoratedName(dynamic input)
        {
            var @params = new[] { (Object)input };
            LogFunctionCall(@params);

            return new { };
        }
        
        [BiteMethod("FakeMethodToCall", HttpMethod = "PUT")]
        public object AttributeDecoratedName_Put(dynamic input)
        {
            var @params = new[] { (Object)input };
            LogFunctionCall(@params);

            return new { };
        }
        
        [BiteMethod("FakeMethodToCall", HttpMethod = "PATCH")]
        public object AttributeDecoratedName_Patch(dynamic input)
        {
            var @params = new[] { (Object)input };
            LogFunctionCall(@params);

            return new { };
        }

        [BiteMethod("FakeMethodToCall", HttpMethod = "DELETE")]
        public object AttributeDecoratedName_Delete(dynamic input)
        {
            var @params = new[] { (Object)input };
            LogFunctionCall(@params);

            return new { };
        }

        [BiteMethod("FakeMethodToCall")]
        public object AttributeDecoratedName(Int32 id, String provider, String project)
        {
            LogFunctionCall(id, provider, project);

            return new { };
        }
    }

    public class InvalidAdapterNamingConvention { }
}