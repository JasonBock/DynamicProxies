namespace DynamicProxies.Tests.Types
{
	public class SimpleDataClass
	{
		public SimpleDataClass(int value)
		{
			this.Value = value;
		}

		public int Value { get; private set; }
	}
}
