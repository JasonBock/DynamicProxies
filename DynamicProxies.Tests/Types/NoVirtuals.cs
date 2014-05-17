using System.Diagnostics.CodeAnalysis;

namespace DynamicProxies.Tests.Types
{
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Virtuals")]
	public class NoVirtuals
	{
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void Hook() { }
	}
}
