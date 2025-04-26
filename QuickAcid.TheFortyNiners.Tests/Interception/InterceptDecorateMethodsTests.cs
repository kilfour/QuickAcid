namespace QuickAcid.TheFortyNiners.Tests;

public class InterceptDecorateMethodsTests
{
    public class Question { public virtual int Answer() => 42; }

    [Fact]
    public void Something()
    {
        var flag = false;
        // Intercept
        //     .This<Question>()
        //     .Before(a => a.Answer())
        //     .Do(() => { flag = true; });

        //Assert.True(flag);
    }
}

public static class Intercept
{

}

