using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using QuickAcid.Bolts;
using QuickAcid.TheyCanFade;
using QuickFuzzr;
using QuickFuzzr.UnderTheHood;

namespace QuickAcid;

public static class Script
{
	public static QAcidScript<Acid> Execute(Action action) =>
		state => { action(); return Vessel.AcidOnly(state); };

	public static QAcidScript<Acid> ExecuteIf(Func<bool> predicate, Action action) =>
		state => { if (predicate()) action(); return Vessel.AcidOnly(state); };
	public static QAcidScript<T> Execute<T>(Func<T> func) =>
		state => Vessel.Some(state, func());

	public static QAcidScript<T> ExecuteIf<T>(Func<bool> predicate, Func<T> func) =>
		state => predicate() ? Vessel.Some(state, func()) : Vessel.None<T>(state);

	public static QAcidScript<T> Execute<T>(Generator<T> generator) =>
		state => Vessel.Some(state, generator(state.FuzzState).Value);

	public static QAcidScript<T> ExecuteIf<T>(Func<bool> predicate, Generator<T> generator) =>
		state => predicate() ? Vessel.Some(state, generator(state.FuzzState).Value) : Vessel.None<T>(state);

	public static QAcidScript<T> Choose<T>(params QAcidScript<T>[] scripts)
		=> ChooseInternal(scripts);

	private static QAcidScript<T> ChooseInternal<T>(
		QAcidScript<T>[] scripts,
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0,
		[CallerMemberName] string member = "")
	{
		var key = $"Choose|{Path.GetFileName(file)}:{line}|{member}|{scripts.Length}";
		return state =>
			{
				var index = state.CurrentExecutionContext().Remember(key,
					() => Fuzz.Int(0, scripts.Length)(state.FuzzState).Value, ReportingIntent.Never);
				return scripts[index](state);
			};
	}

	public static QAcidScript<T> ChooseIf<T>(params (Func<bool> condition, QAcidScript<T> script)[] scripts)
		=> ChooseIfInternal(scripts);

	private static QAcidScript<T> ChooseIfInternal<T>(
		 (Func<bool> condition, QAcidScript<T> script)[] scripts,
		[CallerFilePath] string file = "",
		[CallerLineNumber] int line = 0,
		[CallerMemberName] string member = "")
	{
		var key = $"Choose|{Path.GetFileName(file)}:{line}|{member}|{scripts.Length}";
		return state =>
			{
				var validScriptIndexes =
					scripts.Select((a, ix) => (ix, a.condition))
						.Where(a => a.condition())
						.Select(a => a.ix);
				var index = state.CurrentExecutionContext().Remember(key,
					() => Fuzz.ChooseFrom(validScriptIndexes)(state.FuzzState).Value, ReportingIntent.Never);
				var script = scripts[index];
				if (script.condition())
					return scripts[index].script(state);
				return Vessel.None<T>(state);
			};
	}

	public static QAcidScript<T> Stashed<T>(Func<T> func) => typeof(T).FullName!.Stashed(func);

	public static InputBuilder<TTypedInput> Input<TTypedInput>()
		where TTypedInput : Input => new();

	public record InputBuilder<TTypedInput>()
		where TTypedInput : TypedScript
	{
		public QAcidScript<TValue> From<TValue>(Generator<TValue> generator)
			=> TypedScript.MessageFromType(typeof(TTypedInput)).Input(generator);
	}

	public static QAcidScript<Acid> Act<TTypedInput>(Action action)
		where TTypedInput : Act
		=> TypedScript.MessageFromType(typeof(TTypedInput)).Act(action);

	public static ActBuilder<TTypedInput> Act<TTypedInput>()
		where TTypedInput : Act => new();

	public record ActBuilder<TTypedInput>()
		where TTypedInput : TypedScript
	{
		public QAcidScript<TValue> Do<TValue>(Func<TValue> func)
			=> TypedScript.MessageFromType(typeof(TTypedInput)).Act(func);
	}

	public static QAcidScript<Acid> Spec<TTypedInput>(Func<bool> condition)
		where TTypedInput : Spec
		=> TypedScript.MessageFromType(typeof(TTypedInput)).Spec(condition);
}

public record Input : TypedScript;

public record Act : TypedScript;

public record Spec : TypedScript;

public record TypedScript
{
	public static string MessageFromType(Type type) =>
		LowercaseAllLettersExceptTheFirst(PutASpaceBeforeEachCapital(type.Name));

	private static string LowercaseAllLettersExceptTheFirst(string withSpaces)
		=> $"{new string([.. withSpaces.Take(1)])}{new string([.. withSpaces.Skip(1).Select(char.ToLower)])}";

	private static string PutASpaceBeforeEachCapital(string input)
		=> Regex.Replace(input, "(?<!^)([A-Z])", " $1");
}