using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Hosting;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Threading;
using DynamicProxies.Extensions;
using Xunit;

namespace DynamicProxies.Tests
{
	public sealed class PerformanceTests
		: BaseTests
	{
		[Fact]
		public void CreateAndCall()
		{
			this.Initialize();
			var typesToProxy = new Type[] { typeof(Random), typeof(StringWriter),
				typeof(ApplicationActivator), typeof(FormatterConverter), 
				typeof(TrustManagerContext), typeof(CodeAssignStatement),
				typeof(BackgroundWorker), typeof(CookieCollection), 
				typeof(SemaphoreFullException), typeof(Queue<Guid>) };

			var proxyCreationTime = new Stopwatch();
			var proxyCallTime = new Stopwatch();
			var directCallTime = new Stopwatch();

			foreach (Type typeToProxy in typesToProxy)
			{
				var target = Activator.CreateInstance(typeToProxy);
				proxyCreationTime.Start();
				var proxy = target.CreateProxy(this);
				proxyCreationTime.Stop();

				for (var i = 0; i < 5000; i++)
				{
					proxyCallTime.Start();
					proxy.GetHashCode();
					var proxyType = proxy.GetType();
					var proxyToString = proxy.ToString();
					proxyCallTime.Stop();

					directCallTime.Start();
					target.GetHashCode();
					target.GetType();
					target.ToString();
					directCallTime.Stop();

					if (i == 0)
					{
						Console.Out.WriteLine("FullName: " + proxyType.FullName);
						Console.Out.WriteLine("ToString: " + proxyToString);
					}
				}
			}

			Console.Out.WriteLine(DateTime.Now + " - Time");
			Console.Out.WriteLine(proxyCreationTime.Elapsed.TotalSeconds + " - Total proxy creation time");
			Console.Out.WriteLine(proxyCallTime.Elapsed.TotalSeconds + " - Total proxy call time");
			Console.Out.WriteLine(directCallTime.Elapsed.TotalSeconds + " - Total direct call time");
		}
	}
}
