using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using DynamicProxies.Tests.Types;
using Xunit;

namespace DynamicProxies.Tests
{
	public sealed class ByRefGenericArgumentTests
		: BaseTests
	{
		private bool afterInvocation;
		private bool beforeInvocation;

		protected override void OnInitialize()
		{
			this.afterInvocation = false;
			this.beforeInvocation = false;
		}

		[Fact]
		public void ChangeArgumentValueBeforeInvocation()
		{
			this.Initialize();
			this.beforeInvocation = true;
			ByRefGenericArgumentMethod method = Proxy.Create<ByRefGenericArgumentMethod>(
				new ProxyContext(AssemblyBuilderAccess.RunAndSave, true, true), this);
			var argument = "unchanged";
			method.ByRefArgument<string>(2, "value", ref argument);
			Assert.Equal("before", argument);
		}

		[Fact]
		public void ChangeArgumentValueAfterInvocation()
		{
			this.Initialize();
			this.afterInvocation = true;
			var method = Proxy.Create<ByRefGenericArgumentMethod>(this);
			var argument = "unchanged";
			method.ByRefArgument<string>(2, "value", ref argument);
			Assert.Equal("after", argument);
		}

		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
		protected override sealed bool OnBeforeMethodInvocation(MethodBase target, object[] arguments)
		{
			if (this.beforeInvocation)
			{
				arguments[arguments.Length - 1] = "before";
			}

			return true;
		}

		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
		protected override sealed bool OnAfterMethodInvocation(MethodBase target, object[] arguments, ref object returnValue, Exception generatedException)
		{
			if (this.afterInvocation)
			{
				arguments[arguments.Length - 1] = "after";
			}

			return false;
		}
	}
}
