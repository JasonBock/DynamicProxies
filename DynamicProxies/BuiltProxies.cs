using System;
using System.Collections.Generic;

namespace DynamicProxies
{
	internal static class BuiltProxies
	{
		static BuiltProxies()
		{
			BuiltProxies.Mappings = new Dictionary<Type, Type>();
		}

		internal static Dictionary<Type, Type> Mappings { get; set; }
	}
}
