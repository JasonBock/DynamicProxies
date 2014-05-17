using System;
using System.Reflection;
using System.Reflection.Emit;
using EmitDebugging;

namespace DynamicProxies
{
	internal static class ProxyTypeBuilder
	{
		internal static TypeBuilder Build(ModuleBuilder proxyModule, Type proxyType,
			 AssemblyDebugging debugAssembly)
		{
			TypeBuilder proxyTypeBuilder = null;
			string proxyName = proxyType.Namespace + "." +
				proxyType.Name + Proxy.ProxyExtension;

			var iProxyType = typeof(IProxy<>).MakeGenericType(proxyType);

			proxyTypeBuilder = proxyModule.DefineType(proxyName,
				 TypeAttributes.Class | TypeAttributes.Sealed |
				 TypeAttributes.Public, proxyType, new Type[] { iProxyType });

			using (TypeDebugging debugType = debugAssembly.GetTypeDebugging(proxyTypeBuilder))
			{
				var fields = ProxyFieldBuilder.Build(
					 proxyTypeBuilder, proxyType);
				ProxyConstructorBuilder.Build(proxyTypeBuilder, proxyType,
					 fields[ProxyFieldBuilder.WrappedObjectField],
					 fields[ProxyFieldBuilder.InvokeHandlerField], debugType);
				ProxyMethodBuilder.Build(proxyTypeBuilder, proxyType, iProxyType,
					 fields[ProxyFieldBuilder.WrappedObjectField],
					 fields[ProxyFieldBuilder.InvokeHandlerField], debugType);
			}

			return proxyTypeBuilder;
		}
	}
}
