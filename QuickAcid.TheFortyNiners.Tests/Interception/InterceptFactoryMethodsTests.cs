using QuickAcid.TheFortyNiners.Tests.Interception.Model;

namespace QuickAcid.TheFortyNiners.Tests;

public class InterceptFactoryMethodsTests
{
    [Fact]
    public void Goods_StartsWith_FlagFalse()
    {
        var factory = Decorate
            .This(new Factory())
            .Observe(a => a.Manufacture())
            .Target(a => a.DoStuff())
            .With(a => new SetFlagInterceptor(a));

        var goods = factory.Manufacture();

        Assert.False(goods.Flag);
    }

    [Fact]
    public void DoStuff_Sets_FlagTrue()
    {
        var factory = Decorate
            .This(new Factory())
            .Observe(a => a.Manufacture())
            .Target(a => a.DoStuff())
            .With(a => new SetFlagInterceptor(a));

        var goods = factory.Manufacture();
        goods.DoStuff();

        Assert.True(goods.Flag);
    }

    [Fact]
    public void Factory_Intercepts_NewInstances_Consistently()
    {
        var factory = Decorate
            .This(new Factory())
            .Observe(a => a.Manufacture())
            .Target(a => a.DoStuff())
            .With(a => new SetFlagInterceptor(a));

        var goods1 = factory.Manufacture();
        goods1.DoStuff();
        Assert.True(goods1.Flag);

        var goods2 = factory.Manufacture();
        Assert.False(goods2.Flag);

        goods2.DoStuff();
        Assert.True(goods2.Flag);
    }
}

