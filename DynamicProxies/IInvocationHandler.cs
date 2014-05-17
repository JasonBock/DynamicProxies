using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DynamicProxies
{
	public interface IInvocationHandler
	{
		bool BeforeMethodInvocation(MethodBase target, object[] arguments);
		[SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#")]
		bool AfterMethodInvocation(MethodBase target, object[] arguments,
			ref object returnValue, Exception generatedException);
	}
}
