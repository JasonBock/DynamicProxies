using System;
using System.Diagnostics.CodeAnalysis;

namespace DynamicProxies.Tests.Types
{
	public class ByRefValueType
	{
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
		public virtual void HookByRefGuid(ref Guid arg) { }

		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
		public virtual void HookByRefSByte(ref sbyte arg) { }

		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
		public virtual void HookByRefShort(ref short arg) { }

		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
		public virtual void HookByRefLong(ref long arg) { }

		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
		public virtual void HookByRefLong(ref float arg) { }

		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
		public virtual void HookByRefDouble(ref double arg) { }

		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
		public virtual void HookByRefByte(ref byte arg) { }

		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
		public virtual void HookByRefUShort(ref ushort arg) { }

		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
		public virtual void HookByRefUInt(ref uint arg) { }

		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
		public virtual void HookByRefInt(ref int arg) { }
	}
}
