using System;
using System.Diagnostics.CodeAnalysis;

namespace DynamicProxies.Tests.Types
{
	public class DifferentArgumentCombinations
	{
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Val")]
		public virtual void HookWithByValArgument(string arg1) { }

		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
		public virtual void HookWithByRefArgument(ref string arg1) { }

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Params")]
		public virtual void HookWithParamsArgument(int arg1, params Guid[] arg2) { }
	}
}
