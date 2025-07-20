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

	public T StoreTracked<T>(string key, Func<T> factory)
		=> trackedInputMemory.Store(key, factory);

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

	private readonly Stack<MemoryLens> swappers = new();

	public IDisposable NestedValue(MemoryLens swapper)
	{
		swappers.Push(swapper);
		return new DisposableAction(() => swappers.Pop());
	}

	public IDisposable ScopedSwap(string key, object value)
	{
		var memory = ForThisExecution();

		Func<object> getValue = () => memory.Get<object>(key);
		var setSteps = new Stack<(MemoryLens lens, object container)>();

		foreach (var lens in swappers.Reverse())
		{

			var currentGet = getValue;
			getValue = () => lens.Get(currentGet());

			var container = currentGet(); // capture correct container for this lens
			setSteps.Push((lens, container));
		}

		var oldValue = getValue();

		Func<object, object> setValue = newLeaf =>
		{
			var updated = newLeaf;
			foreach (var (lens, container) in setSteps)
				updated = lens.Set(container, updated);

			return updated;
		};

		memory.Set(key, setValue(value), ReportingIntent.Never);

		return new DisposableAction(() =>
		{
			memory.Set(key, setValue(oldValue), ReportingIntent.Shrinkable);
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
