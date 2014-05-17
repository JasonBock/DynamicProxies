using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using EmitDebugging;

namespace DynamicProxies
{
	internal static class ProxyMethodBuilder
	{
		private const string AfterInvocation = "AfterMethodInvocation";
		private const string AssemblyProperty = "Assembly";
		private const string BeforeInvocation = "BeforeMethodInvocation";
		private const string FullNameProperty = "FullName";
		private const string GetMethodMethod = "GetMethod";
		private const string GetMethodFromHandleMethod = "GetMethodFromHandle";
		private const string GetNameMethod = "GetName";
		private const string GetTargetMethod = "get_Target";
		private const string GetTypeMethod = "GetType";
		private const string GetTypeFromHandle = "GetTypeFromHandle";
		private const string InnerExceptionProperty = "InnerException";
		private const string InvokeMethod = "Invoke";
		private const string InvokeMethodMethod = "InvokeMethod";
		private const string OnAfterMethodInvocationMethod = "OnAfterMethodInvocation";
		private const string OnBeforeMethodInvocationMethod = "OnBeforeMethodInvocation";
		private const string TargetProperty = "Target";

		internal static void Build(TypeBuilder proxyBuilder, Type targetType, Type iProxyType,
			 FieldBuilder wrappedObject, FieldBuilder invokeHandlers, TypeDebugging debug)
		{
			ProxyMethodBuilder.BuildIProxyTargetPropertyMethod(
				proxyBuilder, targetType, iProxyType, wrappedObject, debug);
			MethodBuilder onBeforeMethodInvocation =
				ProxyMethodBuilder.BuildOnBeforeMethodInvocation(proxyBuilder, invokeHandlers, debug);
			MethodBuilder onAfterMethodInvocation =
				ProxyMethodBuilder.BuildOnAfterMethodInvocation(proxyBuilder, invokeHandlers, debug);
			ProxyMethodBuilder.BuildTargetMethods(proxyBuilder, onBeforeMethodInvocation,
				onAfterMethodInvocation, targetType, wrappedObject,
				ProxyMethodBuilder.GetTargetMethods(targetType), debug);
		}

		private static MethodBuilder BuildOnBeforeMethodInvocation(
			TypeBuilder proxyBuilder, FieldBuilder invokeHandlers, TypeDebugging debug)
		{
			var arguments = new Type[] { typeof(MethodBase), typeof(object[]) };

			var method = proxyBuilder.DefineMethod(OnBeforeMethodInvocationMethod,
				MethodAttributes.Private | MethodAttributes.HideBySig, CallingConventions.HasThis,
				typeof(bool), arguments);

			method.DefineParameter(1, ParameterAttributes.In, "currentMethod");
			method.DefineParameter(2, ParameterAttributes.In, "arguments");

			using (var generator = debug.GetMethodDebugging(method))
			{
				var callMethod = generator.DeclareLocal(typeof(bool));
				var i = generator.DeclareLocal(typeof(Int32));
				var doNextHandler = generator.DefineLabel();
				var incrementI = generator.DefineLabel();
				var finish = generator.DefineLabel();

				// Initial local setup.
				//  i = 0
				generator.Emit(OpCodes.Ldc_I4_0);
				generator.Emit(OpCodes.Stloc, i);

				// BeforeInvocationHandler Iteration
				//  Call BeforeInvocationHandler on each IL ref
				//  in invokeHandlers.
				generator.MarkLabel(doNextHandler);
				//  Load the next handler.
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, invokeHandlers);
				generator.Emit(OpCodes.Ldloc, i);
				generator.Emit(OpCodes.Ldelem_Ref);
				//  Load the handler's arguments.
				generator.Emit(OpCodes.Ldarg_1);
				generator.Emit(OpCodes.Ldarg_2);
				//  BeforeInvocationHandler.
				generator.Emit(OpCodes.Callvirt,
					typeof(IInvocationHandler).GetMethod(BeforeInvocation));
				generator.Emit(OpCodes.Stloc, callMethod);
				generator.Emit(OpCodes.Ldloc, callMethod);
				//  If the return value == false, stop loop.
				generator.Emit(OpCodes.Brtrue, incrementI);
				generator.Emit(OpCodes.Br, finish);
				//  Increment i.
				generator.MarkLabel(incrementI);
				generator.Emit(OpCodes.Ldloc, i);
				generator.Emit(OpCodes.Ldc_I4_1);
				generator.Emit(OpCodes.Add);
				generator.Emit(OpCodes.Stloc, i);
				//  See if i < invokeHandlers.Length.
				generator.Emit(OpCodes.Ldloc, i);
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, invokeHandlers);
				generator.Emit(OpCodes.Ldlen);
				generator.Emit(OpCodes.Conv_I4);
				generator.Emit(OpCodes.Blt, doNextHandler);
				//  Exit loop.
				generator.MarkLabel(finish);
				//  Put callMethod on the stack and return.
				generator.Emit(OpCodes.Ldloc, callMethod);
				generator.Emit(OpCodes.Ret);
			}

			return method;
		}

		private static MethodBuilder BuildOnAfterMethodInvocation(TypeBuilder proxyBuilder,
			FieldBuilder invokeHandlers, TypeDebugging debug)
		{
			var arguments = new Type[] { typeof(MethodBase), typeof(object[]), 
				Type.GetType("System.Object&"), typeof(Exception) };

			var method = proxyBuilder.DefineMethod(OnAfterMethodInvocationMethod,
				MethodAttributes.Private | MethodAttributes.HideBySig,
				CallingConventions.HasThis,
				typeof(void), arguments);

			method.DefineParameter(1, ParameterAttributes.In, "currentMethod");
			method.DefineParameter(2, ParameterAttributes.In, "arguments");
			method.DefineParameter(3, ParameterAttributes.In | ParameterAttributes.Out, "returnValue");
			method.DefineParameter(4, ParameterAttributes.In, "generatedException");

			using (var generator = debug.GetMethodDebugging(method))
			{
				var i = generator.DeclareLocal(typeof(Int32));
				var throwException = generator.DeclareLocal(typeof(bool));

				var doNextHandlerAfterEx = generator.DefineLabel();
				var incrementIAfterEx = generator.DefineLabel();

				//  Reset i = 0;
				generator.Emit(OpCodes.Ldc_I4_0);
				generator.Emit(OpCodes.Stloc, i);
				generator.MarkLabel(doNextHandlerAfterEx);
				//  Load the next handler.
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, invokeHandlers);
				generator.Emit(OpCodes.Ldloc, i);
				generator.Emit(OpCodes.Ldelem_Ref);
				//  Load the handler's arguments.
				generator.Emit(OpCodes.Ldarg_1);
				generator.Emit(OpCodes.Ldarg_2);
				generator.Emit(OpCodes.Ldarg_3);
				generator.Emit(OpCodes.Ldarg, 4);
				//  AfterInvocationHandler
				generator.Emit(OpCodes.Callvirt,
					typeof(IInvocationHandler).GetMethod(AfterInvocation));
				//  If the return value == true, rethrow generatedException.
				generator.Emit(OpCodes.Stloc, throwException);
				generator.Emit(OpCodes.Ldloc, throwException);
				generator.Emit(OpCodes.Brfalse, incrementIAfterEx);
				generator.Emit(OpCodes.Ldarg, 4);
				generator.Emit(OpCodes.Throw);
				//  Increment i.
				generator.MarkLabel(incrementIAfterEx);
				generator.Emit(OpCodes.Ldloc, i);
				generator.Emit(OpCodes.Ldc_I4_1);
				generator.Emit(OpCodes.Add);
				generator.Emit(OpCodes.Stloc, i);
				//  See if i < invokeHandlers.Length.
				generator.Emit(OpCodes.Ldloc, i);
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, invokeHandlers);
				generator.Emit(OpCodes.Ldlen);
				generator.Emit(OpCodes.Conv_I4);
				generator.Emit(OpCodes.Blt, doNextHandlerAfterEx);
				//  Exit loop and return
				generator.Emit(OpCodes.Ret);
			}

			return method;
		}

		private static Dictionary<MethodInfo, MethodMappings> GetTargetMethods(Type targetType)
		{
			var targets = new Dictionary<MethodInfo, MethodMappings>();

			foreach (var interfaceOnTarget in targetType.GetInterfaces())
			{
				if (interfaceOnTarget.IsPublic)
				{
					var mapping = targetType.GetInterfaceMap(interfaceOnTarget);

					for (int i = 0; i < mapping.InterfaceMethods.Length; i++)
					{
						MethodInfo trueTarget;

						if (mapping.TargetMethods[i].IsPublic)
						{
							//  We can invoke the true target
							//  so the mapping will be directly to the target method.
							//  Note that I don't care if it's final or not.
							trueTarget = mapping.TargetMethods[i];
						}
						else
						{
							trueTarget = mapping.InterfaceMethods[i];
						}

						MethodMappings methodMapping = null;

						if (!targets.ContainsKey(trueTarget))
						{
							methodMapping = new MethodMappings(true);
							targets.Add(trueTarget, methodMapping);
						}
						else
						{
							methodMapping = targets[trueTarget];
						}

						methodMapping.MappedMethods.Add(mapping.InterfaceMethods[i]);
					}
				}
			}

			foreach (var methodInfo in targetType.GetMethods())
			{
				if (methodInfo.IsPublic && methodInfo.IsVirtual && !methodInfo.IsFinal)
				{
					//  Let's see if we already have it
					//  from the interface mapping.
					MethodMappings baseMethodMapping = null;

					if (!targets.ContainsKey(methodInfo))
					{
						//  This method doesn't override
						//  any itf. methods, so add it.
						baseMethodMapping = new MethodMappings(false);
						targets.Add(methodInfo, baseMethodMapping);
						baseMethodMapping.MappedMethods.Add(methodInfo);
					}
				}
			}

			return targets;
		}

		private static void BuildTargetMethods(TypeBuilder proxyBuilder, MethodBuilder onBeforeMethodInvocation,
			MethodBuilder onAfterMethodInvocation, Type targetType,
			FieldBuilder wrappedObject, Dictionary<MethodInfo, MethodMappings> targets, TypeDebugging debug)
		{
			var methodAttribs = MethodAttributes.HideBySig |
				MethodAttributes.Virtual | MethodAttributes.Private;

			foreach (var target in targets)
			{
				var targetMethod = target.Key;
				var arguments = new Type[targetMethod.GetParameters().Length];

				for (int i = 0; i < targetMethod.GetParameters().Length; i++)
				{
					arguments[i] = targetMethod.GetParameters()[i].ParameterType;
				}

				var method = proxyBuilder.DefineMethod(
					targetMethod.Name + Proxy.ProxyExtension, methodAttribs, targetMethod.ReturnType, arguments);

				ProxyMethodBuilder.HandleGenericMethodArguments(targetMethod, method);

				//  Determine if this method should override
				//  the mapped method (OverridesInterfaceMethods == false)
				//  or a number of itf. methods (OverridesInterfaceMethods == true)
				var mappings = target.Value;

				if (!mappings.OverridesInterfaceMethods)
				{
					proxyBuilder.DefineMethodOverride(method, targetMethod);
				}
				else
				{
					for (int itfs = 0; itfs < mappings.MappedMethods.Count; itfs++)
					{
						proxyBuilder.DefineMethodOverride(method, mappings.MappedMethods[itfs]);
					}
				}

				using (var generator = debug.GetMethodDebugging(method))
				{
					var argumentValues = generator.DeclareLocal(typeof(object[]));
					var callMethod = generator.DeclareLocal(typeof(bool));
					generator.DeclareLocal(typeof(Type));
					var generatedException = generator.DeclareLocal(typeof(Exception));
					LocalBuilder returnValue = null;
					var wrappedReturnValue = generator.DeclareLocal(typeof(object));
					var baseMethod = generator.DeclareLocal(typeof(MethodBase));

					var endCall = generator.DefineLabel();

					//  Check for a return value.
					if (targetMethod.ReturnType != typeof(void))
					{
						returnValue = generator.DeclareLocal(targetMethod.ReturnType);
					}

					generator.Emit(OpCodes.Ldnull);
					generator.Emit(OpCodes.Stloc, generatedException);

					generator.Emit(OpCodes.Ldc_I4, targetMethod.GetParameters().Length);
					generator.Emit(OpCodes.Newarr, typeof(object));
					generator.Emit(OpCodes.Stloc, argumentValues);

					// Get the target method.
					generator.Emit(OpCodes.Ldtoken, targetMethod);

					if (targetType.IsGenericType)
					{
						generator.Emit(OpCodes.Ldtoken, targetType);
						generator.Emit(OpCodes.Call,
							typeof(MethodBase).GetMethod(GetMethodFromHandleMethod,
							new Type[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) }));
					}
					else
					{
						generator.Emit(OpCodes.Call,
							typeof(MethodBase).GetMethod(GetMethodFromHandleMethod,
							new Type[] { typeof(RuntimeMethodHandle) }));
					}

					generator.Emit(OpCodes.Stloc, baseMethod);

					// call OnBeforeMethodInvocation.
					ProxyMethodBuilder.WindArguments(targetMethod, generator, argumentValues, true);
					generator.Emit(OpCodes.Ldarg_0);
					generator.Emit(OpCodes.Ldloc, baseMethod);
					generator.Emit(OpCodes.Ldloc, argumentValues);
					generator.Emit(OpCodes.Call, onBeforeMethodInvocation);
					generator.Emit(OpCodes.Stloc, callMethod);
					ProxyMethodBuilder.UnwindArguments(targetMethod, generator, argumentValues, true);

					// If the call should be cancelled, break to the end.
					generator.Emit(OpCodes.Ldloc, callMethod);
					generator.Emit(OpCodes.Brfalse, endCall);

					// Call the real method.
					generator.BeginExceptionBlock();
					generator.Emit(OpCodes.Ldarg_0);
					generator.Emit(OpCodes.Ldfld, wrappedObject);

					for (var argLoad = 0; argLoad < targetMethod.GetParameters().Length; argLoad++)
					{
						generator.Emit(OpCodes.Ldarg, argLoad + 1);
					}

					generator.Emit(OpCodes.Callvirt, targetMethod);

					if (targetMethod.ReturnType != typeof(void))
					{
						generator.Emit(OpCodes.Stloc, returnValue);
					}

					// call OnAfterInvocation
					ProxyMethodBuilder.WindArguments(targetMethod, generator, argumentValues, false);
					generator.Emit(OpCodes.Ldarg_0);
					generator.Emit(OpCodes.Ldloc, baseMethod);
					generator.Emit(OpCodes.Ldloc, argumentValues);
					ProxyMethodBuilder.WrapReturnValue(targetMethod, generator, returnValue, wrappedReturnValue);
					generator.Emit(OpCodes.Ldloc, generatedException);
					generator.Emit(OpCodes.Call, onAfterMethodInvocation);
					ProxyMethodBuilder.UnwrapReturnValue(targetMethod, generator, returnValue, wrappedReturnValue);
					ProxyMethodBuilder.UnwindArguments(targetMethod, generator, argumentValues, false);

					generator.BeginCatchBlock(typeof(Exception));
					// call OnAfterInvocationWithException
					generator.Emit(OpCodes.Stloc, generatedException);
					ProxyMethodBuilder.WindArguments(targetMethod, generator, argumentValues, false);
					generator.Emit(OpCodes.Ldarg_0);
					generator.Emit(OpCodes.Ldloc, baseMethod);
					generator.Emit(OpCodes.Ldloc, argumentValues);
					ProxyMethodBuilder.WrapReturnValue(targetMethod, generator, returnValue, wrappedReturnValue);
					generator.Emit(OpCodes.Ldloc, generatedException);
					generator.Emit(OpCodes.Call, onAfterMethodInvocation);
					ProxyMethodBuilder.UnwrapReturnValue(targetMethod, generator, returnValue, wrappedReturnValue);
					ProxyMethodBuilder.UnwindArguments(targetMethod, generator, argumentValues, false);
					generator.EndExceptionBlock();
					generator.MarkLabel(endCall);

					//  Finally...return.
					if (targetMethod.ReturnType != typeof(void))
					{
						generator.Emit(OpCodes.Ldloc, returnValue);
					}

					generator.Emit(OpCodes.Ret);
				}
			}
		}

		private static OpCode GetIndCode(Type valueType, bool isLd)
		{
			if (valueType == typeof(sbyte))
			{
				return isLd ? OpCodes.Ldind_I1 : OpCodes.Stind_I1;
			}
			else if (valueType == typeof(short))
			{
				return isLd ? OpCodes.Ldind_I2 : OpCodes.Stind_I2;
			}
			else if (valueType == typeof(int))
			{
				return isLd ? OpCodes.Ldind_I4 : OpCodes.Stind_I4;
			}
			else if (valueType == typeof(long))
			{
				return isLd ? OpCodes.Ldind_I8 : OpCodes.Stind_I8;
			}
			else if (valueType == typeof(float))
			{
				return isLd ? OpCodes.Ldind_R4 : OpCodes.Stind_R4;
			}
			else if (valueType == typeof(double))
			{
				return isLd ? OpCodes.Ldind_R8 : OpCodes.Stind_R8;
			}
			else if (valueType == typeof(byte))
			{
				return isLd ? OpCodes.Ldind_U1 : OpCodes.Stind_I1;
			}
			else if (valueType == typeof(ushort))
			{
				return isLd ? OpCodes.Ldind_U2 : OpCodes.Stind_I2;
			}
			else if (valueType == typeof(uint))
			{
				return isLd ? OpCodes.Ldind_U4 : OpCodes.Stind_I4;
			}
			else
			{
				return isLd ? OpCodes.Ldobj : OpCodes.Stobj;
			}
		}

		private static void UnwindArguments(MethodInfo targetMethod,
			MethodDebugging generator, LocalBuilder argumentValues, bool isBeforeCall)
		{
			if (targetMethod.GetParameters().Length > 0)
			{
				for (var argLoad = 0; argLoad < targetMethod.GetParameters().Length; argLoad++)
				{
					var parameter = targetMethod.GetParameters()[argLoad];
					var parameterType = parameter.ParameterType;

					if (isBeforeCall || parameterType.IsByRef)
					{
						if (parameterType.IsByRef)
						{
							generator.Emit(OpCodes.Ldarg, argLoad + 1);
						}

						generator.Emit(OpCodes.Ldloc, argumentValues);
						generator.Emit(OpCodes.Ldc_I4, argLoad);
						generator.Emit(OpCodes.Ldelem_Ref);

						// This code is odd. By-ref generic parameters are not reported as generic,
						// so that's why the FullName null check is done.
						// Also, the Unbox_Any won't work with by-ref generic arguments, 
						// so the generic type has to be found within the generic arguments array
						// off of the target method.
						if (parameterType.IsGenericParameter || parameterType.FullName == null)
						{
							if (parameterType.IsByRef)
							{
								parameterType = parameterType.GetElementType();
								generator.Emit(OpCodes.Unbox_Any, parameterType);
								generator.Emit(OpCodes.Stobj, parameterType);
							}
							else
							{
								generator.Emit(OpCodes.Unbox_Any, parameterType);
								generator.Emit(OpCodes.Starg, argLoad + 1);
							}
						}
						else
						{
							if (parameterType.IsValueType ||
								(parameterType.HasElementType && parameterType.GetElementType().IsValueType))
							{
								if (parameterType.IsByRef)
								{
									parameterType = parameterType.GetElementType();
									generator.Emit(OpCodes.Unbox_Any, parameterType);

									var indirectCode = ProxyMethodBuilder.GetIndCode(parameterType, false);

									if (indirectCode == OpCodes.Stobj)
									{
										generator.Emit(indirectCode, parameterType);
									}
									else
									{
										generator.Emit(indirectCode);
									}
								}
								else
								{
									generator.Emit(OpCodes.Unbox_Any, parameterType);
									generator.Emit(OpCodes.Starg, argLoad + 1);
								}
							}
							else
							{
								if (parameterType.IsByRef)
								{
									parameterType = parameterType.GetElementType();
									generator.Emit(OpCodes.Castclass, parameterType);
									generator.Emit(OpCodes.Stind_Ref);
								}
								else
								{
									generator.Emit(OpCodes.Castclass, parameterType);
									generator.Emit(OpCodes.Starg, argLoad + 1);
								}
							}
						}
					}
				}
			}
		}

		private static void WindArguments(MethodInfo targetMethod,
			MethodDebugging generator, LocalBuilder argumentValues, bool isBeforeCall)
		{
			//  Set up the arg array
			if (targetMethod.GetParameters().Length > 0)
			{
				for (int argLoad = 0; argLoad < targetMethod.GetParameters().Length; argLoad++)
				{
					var parameter = targetMethod.GetParameters()[argLoad];
					var parameterType = parameter.ParameterType;

					if (isBeforeCall || parameterType.IsByRef)
					{
						generator.Emit(OpCodes.Ldloc, argumentValues);
						generator.Emit(OpCodes.Ldc_I4, argLoad);
						generator.Emit(OpCodes.Ldarg, argLoad + 1);

						if (parameterType.IsGenericParameter || parameterType.FullName == null)
						{
							if (parameterType.IsByRef)
							{
								parameterType = parameterType.GetElementType();
								generator.Emit(OpCodes.Ldobj, parameterType);
								generator.Emit(OpCodes.Box, parameterType);
							}
							else
							{
								generator.Emit(OpCodes.Box, parameterType);
							}
						}
						else if (parameterType.IsValueType ||
							(parameterType.HasElementType && parameterType.GetElementType().IsValueType))
						{
							if (parameterType.IsByRef)
							{
								parameterType = parameterType.GetElementType();

								var indirectCode = ProxyMethodBuilder.GetIndCode(parameterType, true);

								if (indirectCode == OpCodes.Ldobj)
								{
									generator.Emit(indirectCode, parameterType);
								}
								else
								{
									generator.Emit(indirectCode);
								}
							}

							generator.Emit(OpCodes.Box, parameterType);
						}
						else
						{
							if (parameterType.IsByRef)
							{
								generator.Emit(OpCodes.Ldind_Ref);
							}
						}

						generator.Emit(OpCodes.Stelem_Ref);
					}
				}
			}
		}

		private static void HandleGenericMethodArguments(MethodInfo targetMethod, MethodBuilder method)
		{
			if (targetMethod.ContainsGenericParameters)
			{
				var genericArguments = targetMethod.GetGenericArguments();
				var genericParameters = method.DefineGenericParameters(
					Array.ConvertAll<Type, string>(genericArguments,
						new Converter<Type, string>(target => { return target.Name; })));

				for (var i = 0; i < genericParameters.Length; i++)
				{
					var genericParameter = genericParameters[i];
					var genericArgument = genericArguments[i];
					genericParameter.SetGenericParameterAttributes(
						genericArgument.GenericParameterAttributes);
					var interfaceConstraints = new List<Type>();
					Type baseTypeConstraint = null;

					foreach (Type typeConstraint in genericArgument.GetGenericParameterConstraints())
					{
						if (typeConstraint.IsClass)
						{
							baseTypeConstraint = typeConstraint;
						}
						else
						{
							interfaceConstraints.Add(typeConstraint);
						}
					}

					if (baseTypeConstraint != null)
					{
						genericParameter.SetBaseTypeConstraint(baseTypeConstraint);
					}

					if (interfaceConstraints.Count > 0)
					{
						genericParameter.SetInterfaceConstraints(interfaceConstraints.ToArray());
					}
				}
			}

			for (var i = 0; i < targetMethod.GetParameters().Length; i++)
			{
				var parameterInfo = targetMethod.GetParameters()[i];
				method.DefineParameter(i + 1, parameterInfo.Attributes, parameterInfo.Name);
			}
		}

		private static void UnwrapReturnValue(MethodInfo targetMethod, MethodDebugging generator, LocalBuilder returnValue, LocalBuilder wrappedReturnValue)
		{
			if (targetMethod.ReturnType != typeof(void))
			{
				if (targetMethod.ReturnType.IsGenericParameter || targetMethod.ReturnType.IsValueType)
				{
					generator.Emit(OpCodes.Ldloc, wrappedReturnValue);
					generator.Emit(OpCodes.Unbox_Any, targetMethod.ReturnType);
					generator.Emit(OpCodes.Stloc, returnValue);
				}
				else
				{
					generator.Emit(OpCodes.Ldloc, wrappedReturnValue);
					generator.Emit(OpCodes.Castclass, targetMethod.ReturnType);
					generator.Emit(OpCodes.Stloc, returnValue);
				}
			}
		}

		private static void WrapReturnValue(MethodInfo targetMethod, MethodDebugging generator, LocalBuilder returnValue, LocalBuilder wrappedReturnValue)
		{
			if (targetMethod.ReturnType != typeof(void))
			{
				if (targetMethod.ReturnType.IsGenericParameter || targetMethod.ReturnType.IsValueType)
				{
					generator.Emit(OpCodes.Ldloc, returnValue);
					generator.Emit(OpCodes.Box, targetMethod.ReturnType);
					generator.Emit(OpCodes.Stloc, wrappedReturnValue);
					generator.Emit(OpCodes.Ldloca, wrappedReturnValue);
				}
				else
				{
					generator.Emit(OpCodes.Ldloc, returnValue);
					generator.Emit(OpCodes.Castclass, typeof(object));
					generator.Emit(OpCodes.Stloc, wrappedReturnValue);
					generator.Emit(OpCodes.Ldloca, wrappedReturnValue);
				}
			}
			else
			{
				generator.Emit(OpCodes.Ldnull);
				generator.Emit(OpCodes.Stloc, wrappedReturnValue);
				generator.Emit(OpCodes.Ldloca, wrappedReturnValue);
			}
		}

		private static void BuildIProxyTargetPropertyMethod(
			TypeBuilder proxyBuilder, Type targetType, Type iProxyType,
			FieldBuilder wrappedObject, TypeDebugging debug)
		{
			var attributes = MethodAttributes.HideBySig |
				 MethodAttributes.Virtual | MethodAttributes.Private;
			var method = proxyBuilder.DefineMethod(GetTargetMethod,
				 attributes, targetType, Type.EmptyTypes);
			proxyBuilder.DefineMethodOverride(method,
				iProxyType.GetProperty(TargetProperty).GetGetMethod());

			using (var generator = debug.GetMethodDebugging(method))
			{
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, wrappedObject);
				generator.Emit(OpCodes.Ret);
			}
		}
	}
}
