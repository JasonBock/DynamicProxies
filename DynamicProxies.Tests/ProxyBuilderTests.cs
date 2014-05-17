using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using DynamicProxies.Extensions;
using DynamicProxies.Tests.Types;
using Xunit;

namespace DynamicProxies.Tests
{
	public sealed class ProxyBuilderTests
		: BaseTests
	{
		[Fact]
		public void BuildWithCollection()
		{
			this.Initialize();
			var generator1 = new AssemblyGenerator("BuildWithList1", new Dictionary<string, ReadOnlyCollection<MethodGenerator>>()
			{
				{ "Type1", new List<MethodGenerator>()
					{
						new MethodGenerator("Method1", true), 
						new MethodGenerator("Method2", false)
					}.AsReadOnly()
				}, 
				{ "Type2", new List<MethodGenerator>()
					{
						new MethodGenerator("Method1", true), 
						new MethodGenerator("Method2", false)
					}.AsReadOnly()
				}
			});

			var generator2 = new AssemblyGenerator("BuildWithList2", new Dictionary<string, ReadOnlyCollection<MethodGenerator>>()
			{
				{ "Type1", new List<MethodGenerator>()
					{
						new MethodGenerator("Method1", true), 
						new MethodGenerator("Method2", false)
					}.AsReadOnly()
				}, 
			});

			var assembly1 = generator1.Generate();
			var assembly2 = generator2.Generate();

			var types = new List<Type>();
			types.AddRange(assembly1.GetTypes());
			types.AddRange(assembly2.GetTypes());
			ProxyBuilder.Build(types.AsReadOnly());

			var proxyAssemblies = new HashSet<Assembly>();

			foreach (var type1 in assembly1.GetTypes())
			{
				proxyAssemblies.Add(Activator.CreateInstance(type1).CreateProxy(this).GetType().Assembly);
			}

			Assert.Equal(1, proxyAssemblies.Count);
		}

		[Fact]
		public void BuildWithType()
		{
			this.Initialize();
			var generator = new AssemblyGenerator("BuildWithType", new Dictionary<string, ReadOnlyCollection<MethodGenerator>>()
			{
				{ "Type1", new List<MethodGenerator>()
					{
						new MethodGenerator("Method1", true), 
						new MethodGenerator("Method2", false)
					}.AsReadOnly()
				}
			});

			var assembly = generator.Generate();
			ProxyBuilder.Build(assembly.GetTypes()[0]);

			var proxyAssemblies = new HashSet<Assembly>();

			foreach (var type in assembly.GetTypes())
			{
				proxyAssemblies.Add(Activator.CreateInstance(type).CreateProxy(this).GetType().Assembly);
			}

			Assert.Equal(1, proxyAssemblies.Count);
		}

		[Fact]
		public void BuildWithNullContextOnCollection()
		{
			this.Initialize();
			Assert.Throws<ArgumentNullException>(() => ProxyBuilder.Build(new List<Type> { typeof(SimpleClass) }.AsReadOnly(), null));
		}

		[Fact]
		public void BuildWithNullContextOnTarget()
		{
			this.Initialize();
			Assert.Throws<ArgumentNullException>(() => ProxyBuilder.Build(typeof(SimpleClass), null));
		}

		[Fact]
		public void BuildWithEmptyCollection()
		{
			this.Initialize();
			Assert.Throws<ArgumentException>(() => ProxyBuilder.Build(new List<Type>().AsReadOnly()));
		}

		[Fact]
		public void BuildWithNullCollection()
		{
			this.Initialize();
			Assert.Throws<ArgumentNullException>(() => ProxyBuilder.Build((null as ReadOnlyCollection<Type>)));
		}

		[Fact]
		public void BuildWithNullTarget()
		{
			this.Initialize();
			Assert.Throws<ArgumentNullException>(() => ProxyBuilder.Build((null as Type)));
		}

		[Fact]
		public void BuildWithCollectionThatImplementsIProxy()
		{
			this.Initialize();
			Assert.Throws<ArgumentException>(() => ProxyBuilder.Build(new List<Type> { typeof(BadProxyClassWithCorrectTarget) }.AsReadOnly()));
		}

		[Fact]
		public void BuildWithTypeThatImplementsIProxy()
		{
			this.Initialize();
			Assert.Throws<ArgumentException>(() => ProxyBuilder.Build(typeof(BadProxyClassWithCorrectTarget)));
		}
	}
}
