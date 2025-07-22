namespace QuickAcid.Tests._Tools.ThePress;

public abstract class AbstractArticle<T>
{
    protected readonly T deposition;
    public AbstractArticle(T deposition) => this.deposition = deposition;
    public T Read() { return deposition; }
}
