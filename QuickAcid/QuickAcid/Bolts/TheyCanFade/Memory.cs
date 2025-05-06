using QuickAcid.Bolts;
using QuickAcid.MonadiXEtAl;

namespace QuickAcid.Bolts.TheyCanFade;

public class Memory
{
	private Func<int> getCurrentActionId;

	public void SetCurrentActionIdFunction(Func<int> getCurrentActionId)
	{
		this.getCurrentActionId = getCurrentActionId;
	}
	private readonly AlwaysReportedInputMemory alwaysReportedInputMemory;
	private readonly Dictionary<int, Access> memoryPerExecution = [];

	public Memory(Func<int> getCurrentActionId)
	{
		this.getCurrentActionId = getCurrentActionId;
		alwaysReportedInputMemory = new AlwaysReportedInputMemory(getCurrentActionId);
	}

	public T StoreAlwaysReported<T>(string key, Func<T> factory, Func<T, string> stringify)
		=> alwaysReportedInputMemory.Store(key, factory, stringify);

	public T StoreStashed<T>(string key, Func<T> factory)
		=> alwaysReportedInputMemory.StoreWithoutReporting(key, factory);

	// public Maybe<T> RetrieveAlwaysReported<T>(string key)
	// 	=> alwaysReportedInputMemory.Retrieve<T>(key);

	public void ResetRunScopedInputs()
		=> alwaysReportedInputMemory.Reset();

	public Access For(int actionId)
	{
		if (!memoryPerExecution.ContainsKey(actionId))
			memoryPerExecution[actionId] = new Access();
		return memoryPerExecution[actionId];
	}

	public Maybe<Access> TryGet(int actionId)
		=> memoryPerExecution.TryGetValue(actionId, out var access)
			? Maybe<Access>.Some(access)
			: Maybe<Access>.None;

	public IEnumerable<(int actionId, Access access)> AllAccesses()
		=> memoryPerExecution.Select(kvp => (kvp.Key, kvp.Value));

	public IReadOnlyDictionary<int, Dictionary<string, string>> AlwaysReportedSnapshot()
		=> alwaysReportedInputMemory.ReportPerExecutionSnapshot(); // read-only exposure

	public T GetForFluentInterface<T>(string key)
		=> alwaysReportedInputMemory.Retrieve<T>(key)
			.OrElse(For(getCurrentActionId()).GetMaybe<T>(key))
			.Match(
				some: x => x,
				none: () => throw new ThisNotesOnYou(
					$"You're singing in the wrong key. '{key}' wasn't found in AlwaysReported(...) or Fuzzed(...).")
			);

	public Access ForThisExecution()
	{
		var actionId = getCurrentActionId();
		if (!memoryPerExecution.ContainsKey(actionId))
			memoryPerExecution[actionId] = new Access();
		return memoryPerExecution[actionId];
	}

	public IEnumerable<string> GetAllAlwaysReportedKeys() // used by code gen
	{
		return alwaysReportedInputMemory.GetAllAlwaysReportedKeys();
	}

	public Access ForLastExecution() // used by codegen
	{
		return memoryPerExecution.Last().Value;
	}

	public bool Has(int actionId) // used by codegen
		=> memoryPerExecution.ContainsKey(actionId);

	public Func<object, object>? GetNestedValue = null;
	public Func<object, object>? SetNestedValue = null;

	public IDisposable ScopedSwap(object key, object newValue)
	{
		var memory = ForThisExecution();

		object oldValue = null;
		if (GetNestedValue != null)
		{
			oldValue = GetNestedValue(memory.Get<object>(key));
			memory.Set(key, SetNestedValue(newValue), ReportingIntent.Never);
		}
		else
		{
			oldValue = memory.Get<object>(key);
			memory.Set(key, newValue, ReportingIntent.Never);
		}

		return new DisposableAction(() =>
			{
				if (GetNestedValue != null)
					memory.Set(key, SetNestedValue(oldValue), ReportingIntent.Shrinkable);
				else
					memory.Set(key, oldValue, ReportingIntent.Shrinkable);
			});
	}

	// ---------------------------------------------------------------------------------------
	// -- DEEP COPY
	public Memory DeepCopy()
	{
		var newAlwaysReported = alwaysReportedInputMemory.DeepCopy(getCurrentActionId);
		var newMemoryPerExecution = new Dictionary<int, Access>();
		foreach (var kvp in memoryPerExecution)
		{
			newMemoryPerExecution[kvp.Key] = kvp.Value.DeepCopy();
		}
		var newMemory = new Memory(getCurrentActionId, newAlwaysReported, newMemoryPerExecution);
		return newMemory;
	}

	public Memory(
		Func<int> getCurrentActionId,
		AlwaysReportedInputMemory alwaysReportedInputMemory,
		Dictionary<int, Access> memoryPerExecution)
	{
		this.getCurrentActionId = getCurrentActionId;
		this.alwaysReportedInputMemory = alwaysReportedInputMemory;
		this.memoryPerExecution = memoryPerExecution;
	}
	// ---------------------------------------------------------------------------------------
}
