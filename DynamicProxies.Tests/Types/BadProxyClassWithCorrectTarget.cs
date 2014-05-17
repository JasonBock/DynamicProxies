namespace DynamicProxies.Tests.Types
{
	public class BadProxyClassWithCorrectTarget
		: IProxy<BadProxyClassWithCorrectTarget>
	{
		public BadProxyClassWithCorrectTarget Target
		{
			get
			{
				return null;
			}
		}
	}
}
