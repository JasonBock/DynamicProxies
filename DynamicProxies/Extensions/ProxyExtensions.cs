using System;

namespace DynamicProxies.Extensions
{
	public static class ProxyExtensions
	{
		private const string ErrorHandlerArrayIsNull = "The handler array cannot be null.";
		private const string ErrorIProxyImplementation = "The target already implements IProxy<>, which is not allowed.";
		private const string ErrorNoHandlers = "No handlers were provided.";
		private const string ErrorTargetObjectIsNull = "The base object cannot be null.";
		private const string ErrorTargetObjectIsSealed = "The base object cannot be sealed.";

		public static T CreateProxy<T>(this T @this, params IInvocationHandler[] handlers)
			 where T : class
		{
			return @this.CreateProxy(new ProxyContext(), handlers);
		}

		public static T CreateProxy<T>(this T @this, ProxyContext context, params IInvocationHandler[] handlers)
			 where T : class
		{
			@this.CheckCreatePreconditions(handlers);

			var targetType = @this.GetType();
			ProxyBuilder.Build(targetType, context);
			var arguments = new object[] { @this, handlers };

			return Activator.CreateInstance(BuiltProxies.Mappings[targetType], arguments) as T;
		}

		private static void CheckCreatePreconditions<T>(this T @this, IInvocationHandler[] handlers)
			where T : class
		{
			if (@this == null)
			{
				throw new ArgumentNullException("this", ProxyExtensions.ErrorTargetObjectIsNull);
			}

			if (handlers == null)
			{
				throw new ArgumentNullException("handlers", ProxyExtensions.ErrorHandlerArrayIsNull);
			}

			if (handlers.Length == 0)
			{
				throw new ArgumentException(ProxyExtensions.ErrorNoHandlers, "handlers");
			}

			if (@this.GetType().GetInterface("IProxy`1") != null)
			{
				throw new ArgumentException(ProxyExtensions.ErrorIProxyImplementation, "this");
			}
		}
	}
}
