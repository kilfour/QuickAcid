namespace QuickAcid.Bolts;

public record QAcidResult<TValue>(QAcidState State, TValue Value);

public static class QAcidResult
{
	public static QAcidResult<TValue> Some<TValue>(QAcidState state, TValue value) =>
		new(state, value);

	public static QAcidResult<TValue> None<TValue>(QAcidState state) =>
		new(state, default!);

	public static QAcidResult<Acid> AcidOnly(QAcidState state) =>
		Some(state, Acid.Test);

	public static bool IsSome<T>(QAcidResult<T> result) => result.Value is not null;

	public static bool IsNone<T>(QAcidResult<T> result) => result.Value is null;
}
