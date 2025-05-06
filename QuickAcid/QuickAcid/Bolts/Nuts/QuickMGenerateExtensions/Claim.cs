using QuickMGenerate;
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

	public static implicit operator Generator<T>(ClaimedGenerator<T> wrapper)
		=> wrapper._inner;
}


public static class MGenExtensions
{
	public static Generator<T> ChooseFromFrozen<T>(IEnumerable<T> source)
	{
		var frozenList = source.ToList(); // <- Freeze here

		if (!frozenList.Any())
			throw new ArgumentException("ChooseFromFrozen requires at least one item.");

		return MGen.Int(0, frozenList.Count).Select(index => frozenList[index]);
	}
}