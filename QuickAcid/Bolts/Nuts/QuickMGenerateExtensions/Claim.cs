using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Bolts.Nuts.QuickMGenerateExtensions;

public interface IKnowMyGuard<T>
{
	Func<T, bool> Guard { get; }
}

public class ClaimedGenerator<T> : IKnowMyGuard<T>
{
	private readonly Generator<T> _inner;
	private readonly Func<T, bool> _guard;

	public ClaimedGenerator(Generator<T> inner, Func<T, bool> guard)
	{
		_inner = inner;
		_guard = guard;
	}

	public Generator<T> AsGenerator() => _inner;

	public Func<T, bool> Guard => _guard;

	// Optional: implicit conversion so it can be used like a delegate
	public static implicit operator Generator<T>(ClaimedGenerator<T> wrapper)
		=> wrapper._inner;
}