using QuickAcid.Bolts;
using QuickFuzzr;
using StringExtensionCombinators;

namespace QuickAcid;


public class Stash<T>
{
    public readonly Dictionary<Guid, T> Warehouse = [];
    public bool IsEmpty => Warehouse.Count == 0;
    public int Count => Warehouse.Count;

    public T Add(T item) { Warehouse[Guid.NewGuid()] = item; return item; }

    public bool Has(Func<T, bool> predicate) => Warehouse.Values.Any(predicate);

    public IEnumerable<T> Where(Func<T, bool> predicate) => Warehouse.Values.Where(predicate);

    public T Update(Func<T, bool> predicate, Func<T, T> update)
    {
        var value = Warehouse.Values.Where(predicate).Single();
        update(value);
        return value;
    }

    public QAcidScript<T> Do(string label, Action<T> action) =>
        Do(label, _ => true, action);

    public QAcidScript<T> Do(string label, Func<T, bool> predicate, Action<T> action) =>
        Do(label, predicate, a => { action(a); return a; });

    public QAcidScript<T> Do(string label, Func<T, T> func) =>
        Do(label, _ => true, func);

    public QAcidScript<T> Do(string label, Func<T, bool> predicate, Func<T, T> func) =>
        state =>
            {
                var valids = Warehouse
                    .Where(a => predicate(a.Value))
                    .Select(a => a)
                    .ToArray();
                var key = label + "---ID---";
                var pick = Fuzz.ChooseFrom(valids)(state.FuzzState).Value.Key;
                state.CurrentExecutionContext().SetIfNotAlreadyThere(key, pick);
                pick = state.CurrentExecutionContext().Get<Guid>(key);
                if (!Warehouse.ContainsKey(pick))
                    return Vessel.None<T>(state);
                var value = Warehouse[pick];
                return label.ActIf(
                    () => valids.Length != 0,
                    () => func(value))(state);
            };
}

