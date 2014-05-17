using System.Diagnostics.CodeAnalysis;

namespace DynamicProxies.Tests.Types
{
	[SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "U")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
	[SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
	public class SimpleGeneric<T, U, V>
	{
		public virtual T ReflectFirstType(T argument)
		{
			return argument;
		}

		public virtual U ReflectSecondType(U argument)
		{
			return argument;
		}

		public virtual V ReflectThirdType(V argument)
		{
			return argument;
		}
	}
}
