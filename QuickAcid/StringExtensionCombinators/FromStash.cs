using QuickAcid;
using QuickAcid.Bolts;
using QuickFuzzr;

namespace StringExtensionCombinators;

public static partial class QAcidCombinators
{
    public static QAcidScript<T> FromStash<T>(this string key, Func<T, bool> predicate) =>
        state =>
            {
                var stash = state.Memory.GetStashed<Stash<T>>(typeof(Stash<T>).FullName!);
                var valids = stash.Warehouse
                    .Where(a => predicate(a.Value))
                    .ToArray();
                var pick = Fuzz.ChooseFrom(valids)(state.FuzzState).Value.Key;
                state.CurrentExecutionContext().SetIfNotAlreadyThere(key, pick);
                pick = state.CurrentExecutionContext().Get<Guid>(key);
                if (!stash.Warehouse.TryGetValue(pick, out T? value))
                    return Vessel.None<T>(state);
                return Vessel.Some(state, value);
            };
}