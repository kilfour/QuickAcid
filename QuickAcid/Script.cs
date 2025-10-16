using System.Runtime.CompilerServices;
using QuickAcid.Bolts;
using QuickAcid.TheyCanFade;
using QuickFuzzr;
using QuickFuzzr.UnderTheHood;
using StringExtensionCombinators;

namespace QuickAcid;

public static partial class Script
{
	// Execute =>
	// Let(...) for value materialization,
	// Do(...) for side-effect actions.
	public static QAcidScript<Acid> Execute(Action action) =>
		state => { action(); return Vessel.AcidOnly(state); };

	public static QAcidScript<T> Execute<T>(Func<T> func) =>
		state => Vessel.Some(state, func());

	public static QAcidScript<Acid> ExecuteIf(Func<bool> predicate, Action action) =>
		state => { if (predicate()) action(); return Vessel.AcidOnly(state); };

	public static QAcidScript<T> ExecuteIf<T>(Func<bool> predicate, Func<T> func) =>
		state => predicate() ? Vessel.Some(state, func()) : Vessel.None<T>(state);

	public static QAcidScript<T> Execute<T>(Generator<T> generator) =>
		state => Vessel.Some(state, generator(state.FuzzState).Value);

	public static QAcidScript<T> ExecuteIf<T>(Func<bool> predicate, Generator<T> generator) =>
		state => predicate() ? Vessel.Some(state, generator(state.FuzzState).Value) : Vessel.None<T>(state);

	// public static QAcidScript<T> Generate<T>(Generator<T> generator) =>
	// 	state => Vessel.Some(state, generator(state.FuzzState).Value);

	public static QAcidScript<T> Tracked<T>(Func<T> func) => typeof(T).Name.Tracked(func);

	public static QAcidScript<T> Stashed<T>(Func<T> func) => typeof(T).FullName!.Stashed(func);

	public static QAcidScript<Stash<T>> StashFor<T>() => typeof(Stash<T>).FullName!.Stashed(() => new Stash<T>());
}
