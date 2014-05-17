using System.Diagnostics.CodeAnalysis;

namespace DynamicProxies.Tests.Types
{
	public class ReturnValues
	{
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int")]
		public virtual int ReflectInt(int arg1)
		{
			return arg1;
		}

		public virtual Person ReflectPerson(Person person)
		{
			return person;
		}
	}
}
