using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DynamicProxies.Tests.Types;
using Xunit;

namespace DynamicProxies.Tests
{
	public sealed class ReferenceArgumentTypeTests
		: BaseTests
	{
		private bool changeBefore;
		private bool changeAfter;

		[Fact]
		public void CallMethodWithByRefReferenceArgumentAndNoChanges()
		{
			this.Initialize();
			this.changeBefore = false;
			this.changeAfter = false;

			var proxy = Proxy.Create<DifferentArgumentCombinations>(this);

			var value = "Not Changed";
			proxy.HookWithByRefArgument(ref value);

			Assert.Equal("Not Changed", value);
		}

		[Fact]
		public void CallMethodWithByRefReferenceArgumentAndChangeValueAfter()
		{
			this.Initialize();
			this.changeBefore = false;
			this.changeAfter = true;

			var proxy = Proxy.Create<DifferentArgumentCombinations>(this);

			var value = "Not Changed";
			proxy.HookWithByRefArgument(ref value);

			Assert.Equal("Changed", value);
		}

		[Fact]
		public void CallMethodWithByRefReferenceArgumentAndChangeValueBefore()
		{
			this.Initialize();
			this.changeBefore = true;
			this.changeAfter = false;

			var proxy = Proxy.Create<DifferentArgumentCombinations>(this);

			var value = "Not Changed";
			proxy.HookWithByRefArgument(ref value);

			Assert.Equal("Changed", value);
		}

		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
		protected override bool OnAfterMethodInvocation(MethodBase target, object[] arguments, ref object returnValue, Exception generatedException)
		{
			if (target.Name == "HookWithByRefArgument" && this.changeAfter)
			{
				arguments[0] = "Changed";
			}

			return false;
		}

		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
		protected override bool OnBeforeMethodInvocation(MethodBase target, object[] arguments)
		{
			if (target.Name == "HookWithByRefArgument" && this.changeBefore)
			{
				arguments[0] = "Changed";
			}

			return true;
		}
	}
}
