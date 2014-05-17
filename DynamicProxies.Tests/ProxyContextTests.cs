using System;
using System.Reflection.Emit;
using Xunit;

namespace DynamicProxies.Tests
{
	public static class ProxyContextTests
	{
		[Fact]
		public static void CreateDefaultContext()
		{
			var context = new ProxyContext();
			Assert.False(context.Verify);
			Assert.False(context.GenerateDebugging);
			Assert.True((context.Access & AssemblyBuilderAccess.Run) == AssemblyBuilderAccess.Run);
			Assert.False((context.Access & AssemblyBuilderAccess.Save) == AssemblyBuilderAccess.Save);
		}

		[Fact]
		public static void CreateSpecificContext()
		{
			var context = new ProxyContext(
				 AssemblyBuilderAccess.RunAndSave, true, true);
			Assert.True(context.Verify);
			Assert.True(context.GenerateDebugging);
			Assert.True((context.Access & AssemblyBuilderAccess.Run) == AssemblyBuilderAccess.Run);
			Assert.True((context.Access & AssemblyBuilderAccess.Save) == AssemblyBuilderAccess.Save);
		}

		[Fact]
		public static void CreateNonVerifiableContext()
		{
			var context = new ProxyContext(AssemblyBuilderAccess.RunAndSave, false, false);
			Assert.False(context.Verify);
			Assert.False(context.GenerateDebugging);
			Assert.True((context.Access & AssemblyBuilderAccess.Run) == AssemblyBuilderAccess.Run);
			Assert.True((context.Access & AssemblyBuilderAccess.Save) == AssemblyBuilderAccess.Save);
		}

		[Fact]
		public static void CreateVerifiableContextWithTransientAccess()
		{
			Assert.Throws<ArgumentException>(() => new ProxyContext(AssemblyBuilderAccess.Run, true, false));
		}

		[Fact]
		public static void CreateReflectionOnlyContext()
		{
			Assert.Throws<ArgumentException>(() => new ProxyContext(AssemblyBuilderAccess.ReflectionOnly, true, false));
		}

		[Fact]
		public static void CreateSaveOnlyAccessContext()
		{
			Assert.Throws<ArgumentException>(() => new ProxyContext(AssemblyBuilderAccess.Save, true, false));
		}
	}
}
