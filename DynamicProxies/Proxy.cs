using DynamicProxies.Extensions;

namespace DynamicProxies
{
	public static class Proxy
	{
		internal const string ProxyExtension = "Proxy";

		public static T Create<T>(params IInvocationHandler[] handlers)
			 where T : class, new()
		{
			return new T().CreateProxy(new ProxyContext(), handlers);
		}

		public static T Create<T>(ProxyContext context, params IInvocationHandler[] handlers)
			 where T : class, new()
		{
			return new T().CreateProxy(context, handlers);
		}
	}
}
