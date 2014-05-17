using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using AssemblyVerifier;
using EmitDebugging;

namespace DynamicProxies
{
	public static class ProxyBuilder
	{
		private const string ErrorIProxyImplementation = "The target already implements IProxy<>, which is not allowed.";
		private const string ErrorProxyContextIsNull = "The proxy context cannot be null.";
		private const string ErrorTargetListIsEmpty = "The target list must contain elements.";
		private const string ErrorTargetListIsNull = "The target list cannot be null.";
		private const string ErrorTargetObjectIsNull = "The base object cannot be null.";
		private const string ErrorTargetObjectIsSealed = "The base object cannot be sealed.";

		public static void Build(Type target)
		{
			target.CheckBuildPreconditions();
			var context = new ProxyContext();
			var items = ProxyBuilder.Generate(target, context);
			ProxyBuilder.Build(target, items);
			ProxyBuilder.Save(items.Assembly, context);
		}

		public static void Build(Type target, ProxyContext context)
		{
			target.CheckBuildPreconditions();
			context.CheckBuildPreconditions();
			var items = ProxyBuilder.Generate(target, context);
			ProxyBuilder.Build(target, items);
			ProxyBuilder.Save(items.Assembly, context);
		}

		internal static void Build(Type target, ProxyBuilderGeneratorItems items)
		{
			Type proxyType = null;

			if (!BuiltProxies.Mappings.ContainsKey(target))
			{
				using (var debugAssembly = new AssemblyDebugging(items.Assembly.GetName().Name + ".il",
					items.Assembly, items.SymbolDocumentWriter))
				{
					proxyType = ProxyTypeBuilder.Build(
						 items.Module, target, debugAssembly).CreateType();
				}

				BuiltProxies.Mappings.Add(target, proxyType);
			}
		}

		public static void Build(ReadOnlyCollection<Type> targets, ProxyContext context)
		{
			targets.CheckBuildPreconditions();
			context.CheckBuildPreconditions();

			foreach (var target in targets)
			{
				target.CheckBuildPreconditions();
			}

			var nonProxiedTypes = new List<Type>(
				targets.Where(target =>
				{
					return !BuiltProxies.Mappings.ContainsKey(target);
				})
			);

			if (nonProxiedTypes.Count > 0)
			{
				var items = ProxyBuilder.Generate(targets[0], context);

				foreach (var target in targets)
				{
					ProxyBuilder.Build(target, items);
				}

				ProxyBuilder.Save(items.Assembly, context);
			}
		}

		public static void Build(ReadOnlyCollection<Type> targets)
		{
			ProxyBuilder.Build(targets, new ProxyContext());
		}

		private static void CheckBuildPreconditions(this ProxyContext @this)
		{
			if (@this == null)
			{
				throw new ArgumentNullException("this", ProxyBuilder.ErrorProxyContextIsNull);
			}
		}

		private static void CheckBuildPreconditions(this ReadOnlyCollection<Type> @this)
		{
			if (@this == null)
			{
				throw new ArgumentNullException("this", ProxyBuilder.ErrorTargetListIsNull);
			}

			if (@this.Count == 0)
			{
				throw new ArgumentException(ProxyBuilder.ErrorTargetListIsEmpty, "this");
			}
		}

		private static void CheckBuildPreconditions(this Type @this)
		{
			if (@this == null)
			{
				throw new ArgumentNullException("this", ProxyBuilder.ErrorTargetObjectIsNull);
			}

			if (@this.IsSealed)
			{
				throw new ArgumentException(ProxyBuilder.ErrorTargetObjectIsSealed, "this");
			}

			if (@this.GetInterface("IProxy`1") != null)
			{
				throw new ArgumentException(ProxyBuilder.ErrorIProxyImplementation, "this");
			}
		}

		private static ProxyBuilderGeneratorItems Generate(Type target, ProxyContext context)
		{
			var proxyAssemblyName = target.Assembly.GetName().Clone() as AssemblyName;
			proxyAssemblyName.Name = target.Namespace + "." +
				target.Name + "." + Proxy.ProxyExtension;

			var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
				proxyAssemblyName, context.Access);

			if (context.GenerateDebugging)
			{
				var debugAttribute = typeof(DebuggableAttribute);
				var debugConstructor = debugAttribute.GetConstructor(
					 new Type[] { typeof(DebuggableAttribute.DebuggingModes) });
				var debugBuilder = new CustomAttributeBuilder(
					 debugConstructor, new object[] { 
						DebuggableAttribute.DebuggingModes.DisableOptimizations | 
						DebuggableAttribute.DebuggingModes.Default });
				assembly.SetCustomAttribute(debugBuilder);
			}

			ModuleBuilder module = null;

			if ((context.Access & AssemblyBuilderAccess.Save) == AssemblyBuilderAccess.Save)
			{
				module = assembly.DefineDynamicModule(assembly.GetName().Name,
					assembly.GetName().Name + ".dll", context.GenerateDebugging);
			}
			else
			{
				module = assembly.DefineDynamicModule(assembly.GetName().Name,
					context.GenerateDebugging);
			}

			ISymbolDocumentWriter symbolDocumentWriter = null;

			if (context.GenerateDebugging)
			{
				symbolDocumentWriter = module.DefineDocument(
					assembly.GetName().Name + ".il", SymDocumentType.Text,
					SymLanguageType.ILAssembly, SymLanguageVendor.Microsoft);
			}

			return new ProxyBuilderGeneratorItems(assembly, module, symbolDocumentWriter);
		}

		private static void Save(AssemblyBuilder assembly, ProxyContext context)
		{
			if ((context.Access & AssemblyBuilderAccess.Save) == AssemblyBuilderAccess.Save)
			{
				assembly.Save(assembly.GetName().Name + ".dll");

				if (context.Verify)
				{
					AssemblyVerification.Verify(assembly);
				}
			}
		}
	}
}
