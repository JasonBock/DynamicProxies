using System.Reflection;
using DynamicProxies.Tests.Types;
using Xunit;

namespace DynamicProxies.Tests
{
	public sealed class CancelCallTests
		: BaseTests
	{
		protected override sealed void OnInitialize()
		{
			this.Counts.Add("Hook", new CallCount());
		}

		[Fact]
		public void CreateAndCancelCall()
		{
			this.Initialize();
			Proxy.Create<SimpleClass>(this).Hook();
			Assert.Equal(0, this.Counts["Hook"].AfterCount);
		}

		protected override sealed bool OnBeforeMethodInvocation(MethodBase target, object[] arguments)
		{
			return false;
		}
	}
}
