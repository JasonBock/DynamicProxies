namespace DynamicProxies.Tests.Types
{
	public class SealedSimpleClass
		: SimpleClass
	{
		public sealed override void Hook()
		{
			base.Hook();
		}
	}
}
