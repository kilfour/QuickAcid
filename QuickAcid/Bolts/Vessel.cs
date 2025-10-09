namespace QuickAcid.Bolts;

public record Vessel<TValue>(QAcidState State, TValue Value);

public static class Vessel
{
	public static Vessel<TValue> Some<TValue>(QAcidState state, TValue value) =>
		new(state, value);

	public static Vessel<TValue> None<TValue>(QAcidState state) =>
		new(state, default!);

	public static Vessel<Acid> AcidOnly(QAcidState state) =>
		Some(state, Acid.Test);

	public static bool IsSome<T>(Vessel<T> result) => result.Value is not null;

	public static bool IsNone<T>(Vessel<T> result) => result.Value is null;
}
