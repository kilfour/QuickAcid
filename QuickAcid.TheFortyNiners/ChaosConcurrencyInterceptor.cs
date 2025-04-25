using Castle.DynamicProxy;

namespace QuickAcid.TheFortyNiners;

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
