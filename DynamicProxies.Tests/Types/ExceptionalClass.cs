using System;

namespace DynamicProxies.Tests.Types
{
	public class ExceptionalClass
	{
		public virtual void Safe() { }

		public virtual void Unsafe()
		{
			throw new NotImplementedException();
		}
	}
}
