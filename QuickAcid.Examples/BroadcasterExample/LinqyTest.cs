using System.Reflection;
using Castle.DynamicProxy;
using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Examples.BroadcasterExample.SimpleModel;
using QuickMGenerate;

namespace QuickAcid.Examples.BroadcasterExample;

public partial class LinqyTest
{

    //[Fact(Skip = "WIP")]
    [Fact]
    public void CheckItAgain()
    {
        var run =
            from clientProxyFactory in "ClientProxyFactory".Stashed(() => new TestClientProxyFactory())
            from broadcaster in "Broadcaster".Stashed(
                //() => Spread.Chaos<Broadcaster>([clientProxyFactory]).On())
                () => new Broadcaster(clientProxyFactory))
            from needler in "Needler".Stashed(() => new Needler())
            from _o1 in "ops".Choose(

                    from _a1 in "Register Client".Act(broadcaster.Register)
                    from _s1 in "Client Exists In Collection".Spec(
                            () => GetBroadcastersClients(broadcaster).Contains(clientProxyFactory.CreatedClients.Last()))
                    select Acid.Test,

                    // from faultyClient in "Faulty Client".DynamicInput(MGen.ChooseFromWithDefaultWhenEmpty(GetBroadcastersClients(broadcaster)))
                    // from _a2 in "Registered Client Faults".ActIf(() => faultyClient != null,
                    //     () => ((TestClientProxy)faultyClient).Fault())
                    // from _s2 in "Client Is Removed From Collection".Spec(() => !GetBroadcastersClients(broadcaster).Contains(faultyClient))
                    // select Acid.Test,

                    from _a3 in "Broadcast".ActIf(
                        () => !needler.ThreadSwitch,
                        () => needler.Start(() => broadcaster.Broadcast(new Notification())))
                    from _s3 in "Start Does Not Throw".Spec(() => needler.ExceptionFromThread == null)
                    select Acid.Test,

                    from _a4 in "Stop Broadcasting".ActIf(() => needler.ThreadSwitch, needler.Stop)
                    from _s4 in "Stop Does Not Throw".Spec(() => needler.ExceptionFromThread == null)
                    select Acid.Test
            )
            select Acid.Test;

        run.Verify(10, 50);
    }

    private static List<IClientProxy> GetBroadcastersClients(Broadcaster caster)
    {
        var clientsFieldInfo =
            typeof(Broadcaster).GetField("clients", BindingFlags.NonPublic | BindingFlags.Instance);
        return (List<IClientProxy>)clientsFieldInfo!.GetValue(caster)!;
    }
}

public class ChaosConcurrencyInterceptor : IInterceptor
{
    private static readonly Random rng = new Random();

    private readonly HashSet<string> targetedMethods;

    public ChaosConcurrencyInterceptor(params string[] methodNames)
    {
        targetedMethods = methodNames.ToHashSet();
    }

    public void Intercept(IInvocation invocation)
    {
        if (targetedMethods.Contains(invocation.Method.Name))
        {
            MaybeYield();
        }

        invocation.Proceed();
    }

    private void MaybeYield()
    {
        var roll = rng.Next(0, 100);

        if (roll < 10) // 10% yield
        {
            Thread.Yield();
        }
        else if (roll < 15) // 5% tiny sleep
        {
            Thread.Sleep(rng.Next(1, 5));
        }
        else if (roll < 18) // 3% longer sleep
        {
            Thread.Sleep(rng.Next(10, 20));
        }
    }
}

public static class Spread
{
    public class Builder<T>
    {
        private object[] constructorArgs;
        public Builder(object[] constructorArgs)
        {
            this.constructorArgs = constructorArgs;
        }

        public T On(params string[] methods)
        {
            return (T)new ProxyGenerator().CreateClassProxy(
                typeof(T),
                constructorArgs,
                new ChaosConcurrencyInterceptor(methods));
        }
    }
    public static Builder<T> Chaos<T>(object[] constructorArgs) { return new Builder<T>(constructorArgs); }
}