using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DynamicProxies.Tests.Types;
using Xunit;

namespace DynamicProxies.Tests
{
	public sealed class ChangeExceptionTests
		: BaseTests
	{
		private bool createAfter;
		private bool createBefore;

		protected override sealed void OnInitialize()
		{
			this.createAfter = false;
			this.createBefore = false;
		}

		[Fact]
		public void CreateExceptionBeforeCall()
		{
			this.Initialize();
			this.createBefore = true;
			Assert.Throws<InjectedException>(() => Proxy.Create<ExceptionalClass>(this).Safe());
		}

		[Fact]
		public void CreateExceptionAfterCall()
		{
			this.Initialize();
			this.createAfter = true;
			Assert.Throws<InjectedException>(() => Proxy.Create<ExceptionalClass>(this).Safe());
		}

		[Fact]
		public void ChangeExceptionAfterCall()
		{
			this.Initialize();
			this.createBefore = true;
			Assert.Throws<InjectedException>(() => Proxy.Create<ExceptionalClass>(this).Unsafe());
		}

		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override sealed bool OnBeforeMethodInvocation(MethodBase target, object[] arguments)
		{
			if (target.Name == "Safe" && this.createBefore)
			{
				throw new InjectedException();
			}

			return true;
		}

		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override sealed bool OnAfterMethodInvocation(MethodBase target, object[] arguments, ref object returnValue, Exception generatedException)
		{
			var throwException = false;

			if (target.Name == "Safe" && this.createAfter)
			{
				throw new InjectedException();
			}
			else if (target.Name == "Unsafe")
			{
				throw new InjectedException();
			}

			return throwException;
		}
	}
}
