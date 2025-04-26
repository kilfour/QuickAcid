using Castle.DynamicProxy;

namespace QuickAcid.TheFortyNiners.Tests.Interception.Model;

public class SetFlagInterceptor : IInterceptor
{
    private readonly string methodName;

    public SetFlagInterceptor(string methodName)
    {
        this.methodName = methodName;
    }

    public void Intercept(IInvocation invocation)
    {
        MaybeIntercept(invocation);
        invocation.Proceed();
    }

    private void MaybeIntercept(IInvocation invocation)
    {
        if (methodName != invocation.Method.Name) return;
        if (invocation.InvocationTarget == null) return;
        var iHaveAFlag = (IHaveAFlag)invocation.InvocationTarget;
        iHaveAFlag.Flag = true;
    }
}