using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;
using AssemblyVerifier;

namespace DynamicProxies.Tests
{
	internal sealed class AssemblyGenerator
	{
		internal AssemblyGenerator(string @namespace, Dictionary<string, ReadOnlyCollection<MethodGenerator>> types)
			: base()
		{
			this.Namespace = @namespace;
			this.Types = types;
		}

		internal Assembly Generate()
		{
			var name = new AssemblyName();
			name.Name = this.Namespace;
			name.Version = new Version(1, 0, 0, 0);
			string fileName = name.Name + ".dll";

			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
				 name, AssemblyBuilderAccess.RunAndSave);

			var moduleBuilder = assemblyBuilder.DefineDynamicModule(name.Name, fileName, false);

			foreach (var type in this.Types)
			{
				var typeBuilder = moduleBuilder.DefineType(
					 this.Namespace + "." + type.Key,
					 TypeAttributes.Class | TypeAttributes.Public,
					 typeof(object));

				AssemblyGenerator.GenerateConstructor(typeBuilder);

				foreach (var method in type.Value)
				{
					AssemblyGenerator.GenerateMethod(typeBuilder, method);
				}

				typeBuilder.CreateType();
			}

			assemblyBuilder.Save(fileName);
			AssemblyVerification.Verify(assemblyBuilder);

			return assemblyBuilder;
		}

		private static void GenerateConstructor(TypeBuilder typeBuilder)
		{
			var constructor = typeBuilder.DefineConstructor(
				 MethodAttributes.Public | MethodAttributes.SpecialName |
				 MethodAttributes.RTSpecialName,
				 CallingConventions.Standard, Type.EmptyTypes);

			var constructorMethod = typeof(object).GetConstructor(Type.EmptyTypes);
			var constructorGenerator = constructor.GetILGenerator();

			constructorGenerator.Emit(OpCodes.Ldarg_0);
			constructorGenerator.Emit(OpCodes.Call, constructorMethod);
			constructorGenerator.Emit(OpCodes.Ret);
		}

		private static void GenerateMethod(TypeBuilder typeBuilder, MethodGenerator method)
		{
			var attributes = MethodAttributes.Public | MethodAttributes.HideBySig;

			if (method.IsVirtual)
			{
				attributes |= MethodAttributes.Virtual | MethodAttributes.NewSlot;
			}

			typeBuilder.DefineMethod(method.Name, attributes).GetILGenerator().Emit(OpCodes.Ret);
		}

		internal Dictionary<string, ReadOnlyCollection<MethodGenerator>> Types { get; private set; }

		internal string Namespace { get; private set; }
	}
}
