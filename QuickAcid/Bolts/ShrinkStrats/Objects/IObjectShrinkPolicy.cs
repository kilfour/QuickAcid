namespace QuickAcid.Bolts.ShrinkStrats.Objects;

public interface IObjectShrinkPolicy
{
    IEnumerable<IObjectShrinkStrategy> GetStrategies();
}
