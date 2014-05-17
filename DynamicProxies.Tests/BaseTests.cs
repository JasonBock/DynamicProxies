using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicProxies.Tests
{
	public abstract class BaseTests
		: IInvocationHandler
	{
		private ProxyContext debugContext = new ProxyContext(
			AssemblyBuilderAccess.RunAndSave, true, true);

		protected void Initialize()
		{
			this.Counts = new Dictionary<string, CallCount>();
			this.OnInitialize();
		}

		protected virtual bool OnBeforeMethodInvocation(MethodBase target, object[] arguments)
		{
			return true;
		}

		[SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#")]
		protected virtual bool OnAfterMethodInvocation(MethodBase target, object[] arguments, ref object returnValue, Exception generatedException)
		{
			return false;
		}

		protected virtual void OnInitialize() { }

		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		public bool BeforeMethodInvocation(MethodBase target, object[] arguments)
		{
			if (this.Counts.ContainsKey(target.Name))
			{
				this.Counts[target.Name].BeforeCount++;
			}

			return this.OnBeforeMethodInvocation(target, arguments);
		}

		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		public bool AfterMethodInvocation(MethodBase target, object[] arguments, ref object returnValue, Exception generatedException)
		{
			if (this.Counts.ContainsKey(target.Name))
			{
				if (generatedException != null)
				{
					this.Counts[target.Name].AfterExceptionCount++;
				}
				else
				{
					this.Counts[target.Name].AfterCount++;
				}
			}

			return this.OnAfterMethodInvocation(target, arguments, ref returnValue, generatedException);
		}

		protected Dictionary<string, CallCount> Counts { get; private set; }

		protected ProxyContext DebugContext
		{
			get
			{
				return this.debugContext;
			}
		}
	}
}
