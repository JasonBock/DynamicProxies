namespace DynamicProxies.Tests
{
	public sealed class CallCount
	{
		public int AfterCount { get; set; }

		public int AfterExceptionCount { get; set; }

		public int BeforeCount { get; set; }
	}
}
