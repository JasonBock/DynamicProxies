using System;
using DynamicProxies.Extensions;
using DynamicProxies.Tests.Types;
using Xunit;

namespace DynamicProxies.Tests
{
	public sealed class StandardExceptionalTests
		: BaseTests
	{
		[Fact]
		public void CreateProxyForNullTargetObject()
		{
			this.Initialize();
			Assert.Throws<ArgumentNullException>(() => (null as SimpleClass).CreateProxy(this));
		}

		[Fact]
		public void CreateProxyForSealedClass()
		{
			this.Initialize();
			Assert.Throws<ArgumentException>(() => Proxy.Create<SealedClass>(this));
		}

		[Fact]
		public void CreateProxyWithEmptyInvocationHandlerArray()
		{
			this.Initialize();
			Assert.Throws<ArgumentException>(() => Proxy.Create<SimpleClass>(new IInvocationHandler[0]));
		}

		[Fact]
		public void CreateProxyWithNullInvocationHandlerArray()
		{
			this.Initialize();
			Assert.Throws<ArgumentNullException>(() => Proxy.Create<SimpleClass>((null as IInvocationHandler[])));
		}
	}
}
