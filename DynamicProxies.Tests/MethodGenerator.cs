namespace DynamicProxies.Tests
{
	internal sealed class MethodGenerator
	{
		internal MethodGenerator(string name, bool isVirtual)
			: base()
		{
			this.Name = name;
			this.IsVirtual = isVirtual;
		}

		internal bool IsVirtual { get; private set; }

		internal string Name { get; private set; }
	}
}
