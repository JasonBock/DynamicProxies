using System;
using System.Reflection;
using DynamicProxies.Tests.Types;
using Xunit;

namespace DynamicProxies.Tests
{
	public sealed class MultipleHandlerTests
		: BaseTests
	{
		[Fact]
		public void CreateProxyWithMultipleHandlers()
		{
			this.Initialize();
			int finalValue = Proxy.Create<ReturnValues>(this, this, this, this).ReflectInt(2);

			Assert.Equal(36, finalValue);
		}

		protected override sealed bool OnBeforeMethodInvocation(MethodBase target, object[] arguments)
		{
			arguments[0] = (int)arguments[0] * 2;
			return true;
		}

		protected override sealed bool OnAfterMethodInvocation(MethodBase target, object[] arguments, ref object returnValue, Exception generatedException)
		{
			returnValue = (int)returnValue + 1;
			return false;
		}
	}
}
