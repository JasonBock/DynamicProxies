using System;
using System.Configuration;
using System.Reflection.Emit;
using DynamicProxies.Configuration;

namespace DynamicProxies
{
	public sealed class ProxyContext
	{
		public ProxyContext()
			: base()
		{
			this.Access = AssemblyBuilderAccess.Run;

			var configuration =
				ConfigurationManager.GetSection("DynamicProxies") as DynamicProxiesConfiguration;

			if (configuration != null)
			{
				var element = configuration.ProxyContext;

				if (element != null)
				{
					this.Access = element.Access;
					this.GenerateDebugging = element.GenerateDebugging;
					this.Verify = element.Verify;
				}
			}
		}

		public ProxyContext(AssemblyBuilderAccess access, bool verify, bool generateDebugging)
			: this()
		{
			if ((access & AssemblyBuilderAccess.Run) != AssemblyBuilderAccess.Run)
			{
				throw new ArgumentException(
					 "Invalid access value - the assembly must have Run access.", "access");
			}

			if (verify && ((access & AssemblyBuilderAccess.Save) != AssemblyBuilderAccess.Save))
			{
				throw new ArgumentException("Verification cannot be done on a transient assembly.",
					 "verify");
			}

			// TODO - I'm not sure if debugging can be done on transient assemblies...
			this.Access = access;
			this.Verify = verify;
			this.GenerateDebugging = generateDebugging;
		}

		public AssemblyBuilderAccess Access { get; private set; }

		public bool GenerateDebugging { get; private set; }

		public bool Verify { get; private set; }
	}
}
