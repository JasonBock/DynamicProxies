namespace DynamicProxies.Tests.Types
{
	public class ClassWithGenericMethods
	{
		public virtual T ReturnArgumentWithClassConstraint<T>(T arg)
			where T : class
		{
			return arg;
		}

		public virtual T ReturnArgument<T>(T arg)
		{
			return arg;
		}

		public virtual T ReturnArgumentInMiddlePosition<T>(int arg1, T arg2, string arg3)
		{
			return arg2;
		}

		public virtual T ReturnArgumentWithBaseClassConstrains<T>(T arg)
			where T : SimpleDataClass
		{
			return arg;
		}

		public virtual T ReturnArgumentWithInheritanceConstrains<T>(T arg)
			where T : IImplementThis
		{
			return arg;
		}
	}
}
