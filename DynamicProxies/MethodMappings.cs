using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DynamicProxies
{
	public sealed class MethodMappings
	{
		public MethodMappings(bool overridesInterfaceMethods)
			: base()
		{
			this.OverridesInterfaceMethods = overridesInterfaceMethods;
			this.MappedMethods = new List<MethodInfo>();
		}

		public bool OverridesInterfaceMethods { get; private set; }

		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public List<MethodInfo> MappedMethods { get; private set; }
	}
}
