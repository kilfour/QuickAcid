using QuickAcid.MonadiXEtAl;

namespace QuickAcid.Bolts.TheyCanFade;

public class Memory
{
	private Func<int> getCurrentExecutionId;

	public void SetCurrentActionIdFunction(Func<int> getCurrentActionId)
	{
		this.getCurrentExecutionId = getCurrentActionId;
	}
	private readonly TrackedInputMemory trackedInputMemory;
	private readonly Dictionary<int, Access> memoryPerExecution = [];

	private readonly Dictionary<int, Dictionary<string, string>> tracesPerExecution = [];

	public Memory(Func<int> getCurrentExecutionId)
	{
		this.getCurrentExecutionId = getCurrentExecutionId;
		trackedInputMemory = new TrackedInputMemory(getCurrentExecutionId);
	}

	public T StoreTracked<T>(string key, Func<T> factory, Func<T, string> stringify)
		=> trackedInputMemory.Store(key, factory, stringify);

	public T StoreStashed<T>(string key, Func<T> factory)
		=> trackedInputMemory.StoreWithoutReporting(key, factory);

	public void ResetRunScopedInputs()
		=> trackedInputMemory.Reset();

	public Access For(int executionId)
	{
		if (!memoryPerExecution.ContainsKey(executionId))
			memoryPerExecution[executionId] = new Access();
		return memoryPerExecution[executionId];
	}

	public Maybe<Access> TryGet(int executionId)
		=> memoryPerExecution.TryGetValue(executionId, out var access)
			? Maybe<Access>.Some(access)
			: Maybe<Access>.None;

	public IEnumerable<(int executionId, Access access)> AllAccesses()
		=> memoryPerExecution.Select(kvp => (kvp.Key, kvp.Value));

	public IReadOnlyDictionary<int, Dictionary<string, string>> TrackedSnapshot()
		=> trackedInputMemory.ReportPerExecutionSnapshot(); // read-only exposure

	public T GetForFluentInterface<T>(string key)
		=> trackedInputMemory.Retrieve<T>(key)
			.OrElse(For(getCurrentExecutionId()).GetMaybe<T>(key))
			.Match(
				some: x => x,
				none: () => throw new ThisNotesOnYou(
					$"You're singing in the wrong key. '{key}' wasn't found in Tracked(...) or Fuzzed(...).")
			);

	public Access ForThisExecution()
	{
		var executionId = getCurrentExecutionId();
		if (!memoryPerExecution.ContainsKey(executionId))
			memoryPerExecution[executionId] = new Access();
		return memoryPerExecution[executionId];
	}

	public Dictionary<string, string> TracesFor(int executionId)
	{
		if (!tracesPerExecution.ContainsKey(executionId))
			tracesPerExecution[executionId] = [];
		return tracesPerExecution[executionId];
	}

	public Dictionary<string, string> TracesForThisExecution()
	{
		return TracesFor(getCurrentExecutionId());
	}

	public IEnumerable<string> GetAllTrackedKeys() // used by code gen
	{
		return trackedInputMemory.GetAllTrackedKeys();
	}

	public Access ForLastExecution() // used by codegen
	{
		return memoryPerExecution.Last().Value;
	}

	public bool Has(int executionId) // used by codegen
		=> memoryPerExecution.ContainsKey(executionId);

	public Func<object, object>? GetNestedValue = null;
	public Func<object, object>? SetNestedValue = null;
	// put the entire swap thing in a class, allowing for Lensing
	// including key value 
	// the call to ShrinkRunReturnTrueIfFailed can then be wrapped in Func<NewClass, bool>
	// and passed around
	public IDisposable ScopedSwap(object key, object value)
	{
		var memory = ForThisExecution();

		object oldValue = null!;
		if (GetNestedValue != null)
		{
			oldValue = GetNestedValue(memory.Get<object>(key));
			memory.Set(key, SetNestedValue!(value), ReportingIntent.Never);
		}
		else
		{
			oldValue = memory.Get<object>(key);
			memory.Set(key, value, ReportingIntent.Never);
		}

		return new DisposableAction(() =>
			{
				if (GetNestedValue != null)
					memory.Set(key, SetNestedValue!(oldValue), ReportingIntent.Shrinkable);
				else
					memory.Set(key, oldValue, ReportingIntent.Shrinkable);
			});
	}

	// ---------------------------------------------------------------------------------------
	// -- DEEP COPY
	public Memory DeepCopy()
	{
		var newTracked = trackedInputMemory.DeepCopy(getCurrentExecutionId);
		var newMemoryPerExecution = new Dictionary<int, Access>();
		foreach (var kvp in memoryPerExecution)
		{
			newMemoryPerExecution[kvp.Key] = kvp.Value.DeepCopy();
		}
		var newMemory = new Memory(getCurrentExecutionId, newTracked, newMemoryPerExecution);
		return newMemory;
	}

	public Memory(
		Func<int> getCurrentActionId,
		TrackedInputMemory alwaysReportedInputMemory,
		Dictionary<int, Access> memoryPerExecution)
	{
		this.getCurrentExecutionId = getCurrentActionId;
		this.trackedInputMemory = alwaysReportedInputMemory;
		this.memoryPerExecution = memoryPerExecution;
	}
	// ---------------------------------------------------------------------------------------
}
