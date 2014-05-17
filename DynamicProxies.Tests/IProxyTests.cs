using System;
using System.Reflection.Emit;
using DynamicProxies.Extensions;
using DynamicProxies.Tests.Types;
using Xunit;

namespace DynamicProxies.Tests
{
	public sealed class IProxyTests
		: BaseTests
	{
		[Fact]
		public void CreateAndVerifyIProxyInterfaceImplementation()
		{
			this.Initialize();
			var simple = new SimpleClass().CreateProxy(
				new ProxyContext(AssemblyBuilderAccess.RunAndSave, true, false), this);
			Assert.True(typeof(IProxy<SimpleClass>).IsAssignableFrom(simple.GetType()));
			Assert.NotNull((simple as IProxy<SimpleClass>).Target);
		}

		[Fact]
		public void CreateWithTypeCorrectlyImplementingIProxy()
		{
			this.Initialize();
			Assert.Throws<ArgumentException>(() => Proxy.Create<BadProxyClassWithCorrectTarget>(this));
		}

		[Fact]
		public void CreateWithTypeIncorrectlyImplementingIProxy()
		{
			this.Initialize();
			Assert.Throws<ArgumentException>(() => Proxy.Create<BadProxyClassWithIncorrectTarget>(this));
		}
	}
}
