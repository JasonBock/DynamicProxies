using System.Diagnostics.CodeAnalysis;

namespace DynamicProxies.Tests.Types
{
	public class SimpleClass
	{
		public virtual void Hook() { }

		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void DoNotHook() { }
	}
}
