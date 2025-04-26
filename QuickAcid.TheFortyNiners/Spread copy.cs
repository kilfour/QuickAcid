using Castle.DynamicProxy;

namespace QuickAcid.TheFortyNiners;

public static class Spreadss
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

    // public void Foo(Expression<Func<T, TProperty>> func)
    // {

    // }
}
