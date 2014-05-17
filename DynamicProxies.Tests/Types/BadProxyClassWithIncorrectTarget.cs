namespace DynamicProxies.Tests.Types
{
	public class BadProxyClassWithIncorrectTarget
		: IProxy<SimpleClass>
	{
		public SimpleClass Target
		{
			get
			{
				return null;
			}
		}
	}
}
