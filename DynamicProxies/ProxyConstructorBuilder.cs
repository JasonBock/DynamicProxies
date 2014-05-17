using System;
using System.Reflection;
using System.Reflection.Emit;
using EmitDebugging;

namespace DynamicProxies
{
	internal static class ProxyConstructorBuilder
	{
		internal static void Build(TypeBuilder proxyBuilder, Type proxyType,
			 FieldBuilder wrappedObject, FieldBuilder invokeHandlers, TypeDebugging debug)
		{
			ProxyConstructorBuilder.BuildParameterlessConstructor(
				proxyBuilder, proxyType, debug);
			ProxyConstructorBuilder.BuildConstructor(
				proxyBuilder, proxyType, wrappedObject, invokeHandlers, debug);
		}

		private static void BuildConstructor(TypeBuilder proxyBuilder, Type proxyType,
			 FieldBuilder wrappedType, FieldBuilder invokeHandlers, TypeDebugging debug)
		{
			var arguments = new Type[] { proxyType, typeof(IInvocationHandler[]) };
			var constructor = proxyBuilder.DefineConstructor(
				MethodAttributes.Public | MethodAttributes.SpecialName |
				MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
				CallingConventions.Standard, arguments);

			using (var generator = debug.GetMethodDebugging(constructor))
			{
				//  Call the base constructor.
				generator.Emit(OpCodes.Ldarg_0);
				var objectCtor = proxyType.GetConstructor(Type.EmptyTypes);
				generator.Emit(OpCodes.Call, objectCtor);
				//  Store the target object.
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldarg_1);
				generator.Emit(OpCodes.Stfld, wrappedType);
				//  Store the handlers.
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldarg_2);
				generator.Emit(OpCodes.Stfld, invokeHandlers);
				generator.Emit(OpCodes.Ret);
			}
		}

		private static void BuildParameterlessConstructor(TypeBuilder proxyBuilder, Type proxyType,
			TypeDebugging debug)
		{
			var constructor = proxyBuilder.DefineConstructor(
				MethodAttributes.Private | MethodAttributes.SpecialName |
				MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
				CallingConventions.Standard, Type.EmptyTypes);

			using (var generator = debug.GetMethodDebugging(constructor))
			{
				generator.Emit(OpCodes.Ldarg_0);
				var objectCtor = proxyType.GetConstructor(Type.EmptyTypes);
				generator.Emit(OpCodes.Call, objectCtor);
				generator.Emit(OpCodes.Ret);
			}
		}
	}
}
