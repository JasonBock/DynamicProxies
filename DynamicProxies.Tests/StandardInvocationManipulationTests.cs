using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DynamicProxies.Tests.Types;
using Xunit;

namespace DynamicProxies.Tests
{
	public sealed class StandardInvocationManipulationTests
		: BaseTests
	{
		[Fact]
		public void ChangeIntReturn()
		{
			this.Initialize();
			var returnValues = Proxy.Create<ReturnValues>(this);
			var returnedInt = returnValues.ReflectInt(22);
			Assert.Equal(44, returnedInt);
		}

		[Fact]
		public void ChangePersonReturn()
		{
			this.Initialize();
			var returnValues = Proxy.Create<ReturnValues>(this);
			var returnedPerson = returnValues.ReflectPerson(new Person("Joe"));
			Assert.Equal("Jim", returnedPerson.Name);
		}

		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override sealed bool OnAfterMethodInvocation(MethodBase target, object[] arguments, ref object returnValue, Exception generatedException)
		{
			if (target.Name == "ReflectInt")
			{
				returnValue = 44;
			}
			else if (target.Name == "ReflectPerson")
			{
				returnValue = new Person("Jim");
			}

			return false;
		}
	}
}
