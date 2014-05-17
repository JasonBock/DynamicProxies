namespace DynamicProxies.Tests.ExplicitInternalInterfaces
{
	public class PublicClass
		: IInternalInterface
	{
		void IInternalInterface.CallMe() { }

		public void CallMe() { }
	}
}
