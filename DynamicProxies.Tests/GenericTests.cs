using System;
using DynamicProxies.Tests.Types;
using Xunit;

namespace DynamicProxies.Tests
{
	public sealed class GenericTests
		: BaseTests, IImplementThis
	{
		[Fact]
		public void CreateWithGenericMethods()
		{
			this.Initialize();
			var generic = Proxy.Create<ClassWithGenericMethods>(this);
			Assert.Equal(2, generic.ReturnArgument<int>(2));
			Assert.Equal("Hello", generic.ReturnArgument<string>("Hello"));
			Assert.Equal(this, generic.ReturnArgumentWithInheritanceConstrains<GenericTests>(this));
			Assert.Equal(2, generic.ReturnArgumentInMiddlePosition<int>(3, 2, "Hello"));
			Assert.Equal("constraint", generic.ReturnArgumentWithClassConstraint<string>("constraint"));
			Assert.Equal(4,
				generic.ReturnArgumentWithBaseClassConstrains<SimpleDataClass>(new SimpleDataClass(4)).Value);
		}

		[Fact]
		public void CreateForGenericClosedTypeWithDefaultContext()
		{
			this.Initialize();
			var simple = Proxy.Create<SimpleGeneric<string, int, Guid>>(this);
			Assert.Equal("Hello", simple.ReflectFirstType("Hello"));
			Assert.Equal(33, simple.ReflectSecondType(33));
			var newGuid = Guid.NewGuid();
			Assert.Equal(newGuid, simple.ReflectThirdType(newGuid));
		}

		[Fact]
		public void CreateForGenericClosedType()
		{
			this.Initialize();
			var simple = Proxy.Create<SimpleGeneric<string, int, Guid>>(this);
			Assert.Equal("Hello", simple.ReflectFirstType("Hello"));
			Assert.Equal(33, simple.ReflectSecondType(33));
			Guid newGuid = Guid.NewGuid();
			Assert.Equal(newGuid, simple.ReflectThirdType(newGuid));
		}
	}
}
