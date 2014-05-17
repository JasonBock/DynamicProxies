using System.Diagnostics.SymbolStore;
using System.Reflection.Emit;

namespace DynamicProxies
{
	internal sealed class ProxyBuilderGeneratorItems
	{
		private ProxyBuilderGeneratorItems()
			: base() { }

		internal ProxyBuilderGeneratorItems(AssemblyBuilder assembly, ModuleBuilder module,
			ISymbolDocumentWriter symbolDocumentWriter)
			: this()
		{
			this.Assembly = assembly;
			this.Module = module;
			this.SymbolDocumentWriter = symbolDocumentWriter;
		}

		internal AssemblyBuilder Assembly { get; private set; }

		internal ModuleBuilder Module { get; private set; }

		internal ISymbolDocumentWriter SymbolDocumentWriter { get; private set; }
	}
}
