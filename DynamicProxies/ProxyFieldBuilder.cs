using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicProxies
{
	internal static class ProxyFieldBuilder
	{
		internal const string InvokeHandlerField = "invokeHandlers";
		internal const string WrappedObjectField = "wrappedObject";

		internal static Dictionary<string, FieldBuilder> Build(TypeBuilder proxyBuilder, Type proxyType)
		{
			return new Dictionary<string, FieldBuilder> 
			{
				{ 
					ProxyFieldBuilder.InvokeHandlerField, proxyBuilder.DefineField(
						ProxyFieldBuilder.InvokeHandlerField,
						typeof(IInvocationHandler[]), FieldAttributes.Private) 
				}, 
				{
					ProxyFieldBuilder.WrappedObjectField, proxyBuilder.DefineField(
						ProxyFieldBuilder.WrappedObjectField,
						proxyType, FieldAttributes.Private) 
				}
			};
		}
	}
}
