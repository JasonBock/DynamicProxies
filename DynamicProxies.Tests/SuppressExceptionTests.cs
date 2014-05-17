using System;
using System.Reflection;
using DynamicProxies.Tests.Types;
using Xunit;

namespace DynamicProxies.Tests
{
	public sealed class SuppressExceptionTests
		: BaseTests
	{
		private bool suppress;

		protected override sealed void OnInitialize()
		{
			this.suppress = false;
		}

		[Fact]
		public void DoNotSuppressExceptionAfterCall()
		{
			this.Initialize();
			Assert.Throws<NotImplementedException>(() => Proxy.Create<ExceptionalClass>(this).Unsafe());
		}

		[Fact]
		public void SuppressExceptionAfterCall()
		{
			this.Initialize();
			this.suppress = true;
			Proxy.Create<ExceptionalClass>(this).Unsafe();
		}

		protected override sealed bool OnAfterMethodInvocation(MethodBase target, object[] arguments, ref object returnValue, Exception generatedException)
		{
			return !this.suppress;
		}
	}
}
