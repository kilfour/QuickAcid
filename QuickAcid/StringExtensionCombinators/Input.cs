using QuickAcid.Bolts;
using QuickAcid.Phasers;
using QuickAcid.Shrinking;
using QuickAcid.Shrinking.Custom;
using QuickFuzzr;
using QuickPulse.Show;
using QuickAcid;

namespace StringExtensionCombinators;


public static partial class QAcidCombinators
{
	public record InputConfiguration<T>()
	{
		public bool NeedsTracing { get; init; }
		public InputConfiguration<T> Trace() => this with { NeedsTracing = true };
		public InputConfiguration<T> Trace(Func<bool> predicate) => predicate() ? this with { NeedsTracing = true } : this;

		public bool HasSchrinker { get; init; }
		public Func<T, IEnumerable<T>> Shrinker { get; init; } = _ => [];
		public InputConfiguration<T> ShrinkWith(Func<T, IEnumerable<T>> shrinker)
			=> this with { HasSchrinker = true, Shrinker = shrinker };

		public bool HasReducer { get; init; }
		public Func<T, IEnumerable<T>> Reducer { get; init; } = _ => [];
		public InputConfiguration<T> ReduceWith(Func<T, IEnumerable<T>> reducer)
			=> this with { HasReducer = true, Reducer = reducer };
	};

	public static QAcidScript<T> Input<T>(
		this string key,
		FuzzrOf<T> generator,
		Func<InputConfiguration<T>, InputConfiguration<T>> configAction)
	{
		return state =>
			{
				var cfg = configAction(new InputConfiguration<T>());
				if (cfg.HasSchrinker)
				{
					state.ShrinkingRegistry.Register(new LambdaShrinker<T>(cfg.Shrinker));
				}
				if (cfg.HasReducer)
				{
					state.ReducersRegistry.Register(cfg.Reducer);
				}
				var result = state.HandleInput(key, generator);
				if (cfg.NeedsTracing)
				{
					key.Trace(() => Introduce.This(result.Value!, false))(state);
				}
				if (cfg.HasSchrinker)
				{
					state.ShrinkingRegistry.Remove<T>();
				}
				if (cfg.HasReducer)
				{
					state.ReducersRegistry.Remove<T>();
				}
				return result;
			};
	}

	public static QAcidScript<T> Input<T>(this string key, FuzzrOf<T> generator)
	{
		return state =>
			{
				return state.HandleInput(key, generator);
			};
	}

	private static Vessel<T> HandleInput<T>(this QAcidState state, string key, FuzzrOf<T> generator)
	{
		var execution = state.CurrentExecutionContext();
		switch (state.Shifter.CurrentPhase)
		{
			case QAcidPhase.ShrinkInputEval:
			case QAcidPhase.ShrinkingExecutions:
			case QAcidPhase.ShrinkingActions:
				return execution.ContainsKey(key) ?
					Vessel.Some(state, execution.Get<T>(key))
					: Vessel.None<T>(state);

			case QAcidPhase.ShrinkingInputs
				when execution.AlreadyTriedToShrink(key):
				{
					var value = generator(state.FuzzState).Value;
					execution.SetIfNotAlreadyThere(key, value);
					return Vessel.Some(state, value);
				}

			case QAcidPhase.ShrinkingInputs
				when !execution.AlreadyTriedToShrink(key):
				{
					if (execution.ContainsKey(key))
					{
						var value = execution.Get<T>(key);
						ShrinkStrategyPicker.Input(state, key, value, key);
						execution.MarkAsTriedToShrink(key);
						return Vessel.Some(state, value);
					}
					return Vessel.None<T>(state); // TODO Test with multiple actions, first one throws
				}

			default:
				{
					var value = generator(state.FuzzState).Value;
					execution.SetIfNotAlreadyThere(key, value);
					return Vessel.Some(state, value);
				}
		}
	}
}
