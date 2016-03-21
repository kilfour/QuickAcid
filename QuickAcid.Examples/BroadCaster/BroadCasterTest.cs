using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using QuickAcid.Examples.BroadCaster.SimpleModel;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;
using Rhino.Mocks;
using Xunit;

namespace QuickAcid.Examples.BroadCaster
{
	public class BroadCasterTest
	{
		[Fact]//(Skip="I reckon we have some thread safety issues in QAcidState")]
		public void Acid()
		{
			var allTogether =
				from info in "thread info".OnceOnlyInput(() => new ThreadInfo())
				from clientFactory in "clientFactory".OnceOnlyInput(() => MockRepository.GenerateMock<IClientProxyFactory>())
				from broadcaster in "broadcaster".OnceOnlyInput(() => new Broadcaster(clientFactory))
				from actions in "choose".Choose(
					RegisterClient(clientFactory, broadcaster),
					ClientFaults(broadcaster),
					Broadcast(broadcaster, info),
					StopBroadcasting(broadcaster, info))
				select Unit.Instance;

			allTogether.Verify(100, 20);
		}

		public class ThreadInfo
		{
			public Thread Thread;
			public bool ThreadSwitch;
			public Exception ExceptionFromThread;
		}

		private static QAcidRunner<Unit> RegisterClient(IClientProxyFactory clientFactory, Broadcaster broadcaster)
		{
			return
				from sleepTime in "sleeptime".Input(MGen.Int(0, 10))
				from client in "new client".Input(() => MockRepository.GenerateMock<IClientProxy>())
				from register in "Broadcaster.Register".Act(
				() =>
					{
						clientFactory
							.Stub(f => f.CreateClientProxyForCurrentContext(null))
							.IgnoreArguments()
							.Return(client)
							.Repeat.Once();
						client
							.Stub(c => c.SendNotificationAsynchronously(null))
							.IgnoreArguments()
							.WhenCalled(obj => Thread.Sleep(sleepTime));
						broadcaster.Register();
					})
				from spec in "client is added to broadcaster".Spec(() => GetBroadcastersClients(broadcaster).Contains(client))
				select Unit.Instance;
		}

		private static QAcidRunner<Unit> ClientFaults(Broadcaster broadcaster)
		{
			return
				from canAct in
					"broadcaster has clients".If(
						() => GetBroadcastersClients(broadcaster).Count > 0,
						from client in "faulting client".Input(MGen.ChooseFrom(GetBroadcastersClients(broadcaster)))
						from raise in "raise fault".Act(() => client.Raise(c => c.Faulted += null, client, EventArgs.Empty))
						from spec in"client is removed from broadcaster"
							.Spec(() => !GetBroadcastersClients(broadcaster).Contains(client))
						select Unit.Instance)
				select Unit.Instance;
		}

		private static QAcidRunner<Unit> Broadcast(Broadcaster broadcaster, ThreadInfo info)
		{
			return
				from canact in
					"when threadswitch is false".If(
						() => info.ThreadSwitch == false,
						from start in "start broadcasting".Act(
							() =>
								{
									info.ThreadSwitch = true;
									info.ExceptionFromThread = null;
									info.Thread =
										new Thread(
											() => info.ExceptionFromThread =
											      GetExceptionThrownBy(
											      	() => broadcaster.Broadcast(null)));

									info.Thread.Start();
								})
						from spec in "Broadcast : No Exception is thrown".Spec(() => info.ExceptionFromThread == null)
						select Unit.Instance)
				select Unit.Instance;
		}

		private static QAcidRunner<Unit> StopBroadcasting(Broadcaster broadcaster, ThreadInfo info)
		{
			return
				from canact in
					"when threadswitch is true".If(
						() => info.ThreadSwitch,
						from stop in "stop broadcasting".Act(
							() =>
								{
									info.Thread.Join();
									info.Thread = null;
									info.ThreadSwitch = false;
								})
						from spec in "StopBroadcasting : No Exception is thrown".Spec(() => info.ExceptionFromThread == null)
						select Unit.Instance)
				select Unit.Instance;
		}

		private static Exception GetExceptionThrownBy(Action yourCode)
		{
			try { yourCode(); }
			catch (Exception e) { return e; }
			return null;
		}

		public static List<IClientProxy> GetBroadcastersClients(Broadcaster broadcaster)
		{
			var clientsFieldInfo =
				typeof(Broadcaster).GetField("clients", BindingFlags.NonPublic | BindingFlags.Instance);
			return (List<IClientProxy>)clientsFieldInfo.GetValue(broadcaster);
		}
	}
}
