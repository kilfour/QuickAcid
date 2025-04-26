using System.Linq.Expressions;
using Castle.DynamicProxy;

namespace QuickAcid.TheFortyNiners;

public static class Decorate
{

    public static DecorateBuilder<T> This<T>(T factory)
    {
        return new DecorateBuilder<T>(factory);
    }
}

public class ObserverBuilder<TParent, T>
{
    private readonly Expression<Func<TParent, T>> selector;

    public ObserverBuilder(Expression<Func<TParent, T>> selector)
    {
        this.selector = selector;
    }

    public ObserverBuilder<TParent, T> Target(Action<T> targetSelector)
    {
        // return (T)new ProxyGenerator().CreateClassProxy(
        //         typeof(T),
        //         new ChaosConcurrencyInterceptor(methods));
        return this;
    }

    public ObserverBuilder<TParent, T> Target(Func<T, object> targetSelector)
    {
        // return (T)new ProxyGenerator().CreateClassProxy(
        //         typeof(T),
        //         new ChaosConcurrencyInterceptor(methods));
        return this;
    }

    public TParent With(Func<string, IInterceptor> interceptorFactory)
    {
        return default(TParent);
    }
}

public class DecorateBuilder<T>
{
    private readonly T factory;
    // private Func<T, object> manufactureSelector;
    // private Func<object, object> targetSelector;
    // private Type interceptorType;

    public DecorateBuilder(T factory)
    {
        this.factory = factory;
    }

    public ObserverBuilder<T, TProperty> Observe<TProperty>(Expression<Func<T, TProperty>> selector)
    {
        return new ObserverBuilder<T, TProperty>(selector);
    }

    // public DecorateBuilder<T> Target(Func<object, object> targetSelector)
    // {
    //     this.targetSelector = targetSelector;
    //     return this;
    // }

    // public DecorateBuilder<T> Intercept<TInterceptor>() where TInterceptor : IInterceptor, new()
    // {
    //     this.interceptorType = typeof(TInterceptor);
    //     return this;
    // }

    public T Build()
    {
        return (T)new ProxyGenerator().CreateClassProxy(typeof(T), new ChaosConcurrencyInterceptor());
    }
}
