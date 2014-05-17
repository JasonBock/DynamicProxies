using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DynamicProxies.Tests
{
	[Serializable]
	[SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]
	internal sealed class InjectedException : Exception
	{
		internal InjectedException()
			: base() { }

		private InjectedException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal InjectedException(string message)
			: base(message) { }

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal InjectedException(string message, Exception innerException)
			: base(message, innerException) { }
	}
}
