using System.Reflection.Emit;
using DynamicProxies.Tests.ExplicitInternalInterfaces;
using DynamicProxies.Tests.Types;
using Xunit;

namespace DynamicProxies.Tests
{
	public sealed class InterfaceImplementationTests
		: BaseTests
	{
		protected override void OnInitialize()
		{
			this.Counts.Add("ImplementMe", new CallCount());
		}

		[Fact]
		public void CreateWithInterfaceImplementation()
		{
			this.Initialize();
			var implementation = Proxy.Create<Implementation>(this);
			implementation.ImplementMe();
			(implementation as IImplementation).ImplementMe();
			implementation.ImplementMe();
			Assert.Equal(1, this.Counts["ImplementMe"].BeforeCount);
		}

		[Fact]
		public void CreateWithExplicitInterfaceImplementationWithInternalInterface()
		{
			this.Initialize();
			var publicClass = Proxy.Create<PublicClass>(
				new ProxyContext(AssemblyBuilderAccess.RunAndSave, true, true), this);
			publicClass.CallMe();
			Assert.Equal(0, this.Counts["ImplementMe"].BeforeCount);
		}

		[Fact]
		public void CreateWithExplicitInterfaceImplementation()
		{
			this.Initialize();
			var implementation = Proxy.Create<ExplicitImplementation>(this);
			(implementation as IImplementation).ImplementMe();
			Assert.Equal(1, this.Counts["ImplementMe"].BeforeCount);
		}
	}
}
