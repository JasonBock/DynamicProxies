using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace DynamicProxies.Client.Windows
{
	public partial class MainForm
		: Form, IInvocationHandler
	{
		private int calls;

		public MainForm()
		{
			InitializeComponent();
		}

		private void OnTestProxyClick(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			this.MakeAndUseNewProxy();
			this.Cursor = Cursors.Default;
		}

		private void MakeAndUseNewProxy()
		{
			this.calls = 0;

			var simple = Proxy.Create<SimpleClass>(
				new ProxyContext(AssemblyBuilderAccess.Run, false, true), this);
			simple.Call();
			simple.Call();
			simple.Call();
			this.callCount.Text = this.calls.ToString();
		}

		public bool BeforeMethodInvocation(MethodBase target, object[] arguments)
		{
			this.calls++;
			return true;
		}

		public bool AfterMethodInvocation(MethodBase target, object[] arguments, ref object returnValue, Exception generatedException)
		{
			return false;
		}
	}
}