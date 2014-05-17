using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DynamicProxies.Extensions;
using DynamicProxies.Tests.InterfaceImplementation;
using DynamicProxies.Tests.Types;
using Xunit;

namespace DynamicProxies.Tests
{
	public sealed class StandardTests
		: BaseTests, IImplementThis
	{
		protected override void OnInitialize()
		{
			this.Counts.Add("Hook", new CallCount());
			this.Counts.Add("Calling", new CallCount());
			this.Counts.Add("Invoke", new CallCount());
		}

		[Fact]
		public void CreateWithDefaultContext()
		{
			this.Initialize();
			var simple = new SimpleClass().CreateProxy(this);
			simple.Hook();
			simple.DoNotHook();
			simple.Hook();

			var hooks = this.Counts["Hook"];
			Assert.Equal(2, hooks.BeforeCount);
			Assert.Equal(2, hooks.AfterCount);
			Assert.Equal(0, hooks.AfterExceptionCount);
		}

		[Fact]
		public void CreateMultipleProxies()
		{
			this.Initialize();
			var proxy = Proxy.Create<SimpleClass>(this);
			Proxy.Create<ReturnValues>(this);
			proxy.Hook();
			Proxy.Create<ExceptionalClass>(this);
		}

		[Fact]
		public void Create()
		{
			this.Initialize();
			var simple = new SimpleClass().CreateProxy(this.DebugContext, this);
			simple.Hook();
			simple.DoNotHook();
			simple.Hook();

			var hooks = this.Counts["Hook"];
			Assert.Equal(2, hooks.BeforeCount);
			Assert.Equal(2, hooks.AfterCount);
			Assert.Equal(0, hooks.AfterExceptionCount);
		}

		[Fact]
		public void CreateViaTypeWithDefaultContext()
		{
			this.Initialize();
			var simple = Proxy.Create<SimpleClass>(this);
			simple.Hook();
			simple.DoNotHook();
			simple.Hook();

			var hooks = this.Counts["Hook"];
			Assert.Equal(2, hooks.BeforeCount);
			Assert.Equal(2, hooks.AfterCount);
			Assert.Equal(0, hooks.AfterExceptionCount);
		}

		[Fact]
		public void CreateProxyWithNoVirtualMembersWithDefaultContext()
		{
			this.Initialize();
			var noVirtuals = Proxy.Create<NoVirtuals>(this);
			noVirtuals.Hook();
			noVirtuals.Hook();

			var hooks = this.Counts["Hook"];
			Assert.Equal(0, hooks.BeforeCount);
			Assert.Equal(0, hooks.AfterCount);
			Assert.Equal(0, hooks.AfterExceptionCount);
		}

		[Fact]
		public void ProxyClassWithOverriddenMethodThatIsSealed()
		{
			this.Initialize();
			var sealedVirtual = Proxy.Create<SealedSimpleClass>(this);
			sealedVirtual.Hook();
			sealedVirtual.Hook();

			var hooks = this.Counts["Hook"];
			Assert.Equal(0, hooks.BeforeCount);
			Assert.Equal(0, hooks.AfterCount);
			Assert.Equal(0, hooks.AfterExceptionCount);
		}

		[Fact]
		public void ProxyClassWithComplexVBInterfaceImplementationMapping()
		{
			this.Initialize();
			var implementor = Proxy.Create<Implementor>(this);
			// These direct calls are not hooked :)
			implementor.Calling();
			implementor.Invoke();
			implementor.Calling();
			implementor.Invoke();

			(implementor as IOne).Invoke();
			(implementor as ITwo).Invoke();
			(implementor as IThree).CallIt();
			(implementor as IFour).Invoke();

			// Invoke count should be 1.
			var invokes = this.Counts["Invoke"];
			Assert.Equal(1, invokes.BeforeCount);
			Assert.Equal(1, invokes.AfterCount);
			Assert.Equal(0, invokes.AfterExceptionCount);

			// Calling could should be 3.
			var callings = this.Counts["Calling"];
			Assert.Equal(3, callings.BeforeCount);
			Assert.Equal(3, callings.AfterCount);
			Assert.Equal(0, callings.AfterExceptionCount);
		}

		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
		protected override sealed bool OnBeforeMethodInvocation(MethodBase target, object[] arguments)
		{
			if (target.Name == "Hook")
			{
				Assert.Equal(0, arguments.Length);
			}

			return true;
		}

		protected override sealed bool OnAfterMethodInvocation(MethodBase target, object[] arguments, ref object returnValue, Exception generatedException)
		{
			return (generatedException != null);
		}
	}
}
