using System.Diagnostics.CodeAnalysis;

namespace DynamicProxies.Tests.Types
{
	public class ByRefGenericArgumentMethod
	{
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#")]
		public virtual void ByRefArgument<T>(int arg1, T arg2, ref T arg3) { }
	}
}
