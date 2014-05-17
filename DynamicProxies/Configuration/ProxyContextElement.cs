using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;

namespace DynamicProxies.Configuration
{
	[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
	internal sealed class ProxyContextElement
		: ConfigurationElement
	{
		private const string AccessName = "Access";
		private const string GenerateDebuggingName = "GenerateDebugging";
		private const string VerifyName = "Verify";

		[ConfigurationProperty(ProxyContextElement.AccessName,
			DefaultValue = AssemblyBuilderAccess.Run, IsRequired = false)]
		internal AssemblyBuilderAccess Access
		{
			get
			{
				return (AssemblyBuilderAccess)this[ProxyContextElement.AccessName];
			}
		}

		[ConfigurationProperty(ProxyContextElement.GenerateDebuggingName, DefaultValue = false, IsRequired = false)]
		internal bool GenerateDebugging
		{
			get
			{
				return (bool)this[ProxyContextElement.GenerateDebuggingName];
			}
		}

		[ConfigurationProperty(ProxyContextElement.VerifyName, DefaultValue = false, IsRequired = false)]
		internal bool Verify
		{
			get
			{
				return (bool)this[ProxyContextElement.VerifyName];
			}
		}
	}
}
