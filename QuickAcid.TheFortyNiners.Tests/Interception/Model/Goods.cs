namespace QuickAcid.TheFortyNiners.Tests.Interception.Model;

public class Goods : IHaveAFlag
{
    public virtual bool Flag { get; set; } = false;
    public virtual void DoStuff() { }
}
