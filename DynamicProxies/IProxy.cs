namespace DynamicProxies
{
	public interface IProxy<T> where T : class
	{
		T Target { get; }
	}
}
