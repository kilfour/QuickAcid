namespace QuickAcid.TheFortyNiners.Tests;

public class InterceptFactoryMethodsTests
{
    [Fact]
    public void Something()
    {

    }
}

public class Factory
{
    public virtual Goods Manufacture() { return new Goods(); }
}

public class Goods
{
    public virtual void DoStuff() { }
}