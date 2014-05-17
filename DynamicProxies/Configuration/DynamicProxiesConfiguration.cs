using System;
using System.Configuration;

namespace DynamicProxies.Configuration
{
	internal sealed class DynamicProxiesConfiguration : ConfigurationSection
	{
		private const string PropertyName = "ProxyContext";

		[ConfigurationProperty(DynamicProxiesConfiguration.PropertyName, IsRequired = false)]
		internal ProxyContextElement ProxyContext
		{
			get
			{
				return this[DynamicProxiesConfiguration.PropertyName] as ProxyContextElement;
			}
		}
	}
}
