using System;

namespace DynamicProxies.Tests.Types
{
	public sealed class Person
	{
		private string name;

		private Person()
			: base() { }

		public Person(string name)
			: this()
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("The name is incorrect.", "name");
			}

			this.name = name;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}
	}
}
