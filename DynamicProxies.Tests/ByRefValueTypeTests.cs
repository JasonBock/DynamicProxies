using System.Reflection;
using DynamicProxies.Tests.Types;
using Xunit;

namespace DynamicProxies.Tests
{
	public sealed class ByRefValueTypeTests
		: BaseTests
	{
		// Need all the value type method call tested.
		[Fact]
		public void CallInt()
		{
			this.Initialize();
			var simple = Proxy.Create<ByRefValueType>(this.DebugContext, this);
			var x = 3;
			simple.HookByRefInt(ref x);
			Assert.Equal(6, x);
		}

		protected override sealed bool OnBeforeMethodInvocation(MethodBase target, object[] arguments)
		{
			arguments[0] = (int)arguments[0] * 2;
			return true;
		}
	}
}
