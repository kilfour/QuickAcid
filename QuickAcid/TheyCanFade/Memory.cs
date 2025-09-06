using QuickAcid;

namespace QuickAcid.TheyCanFade;

public class Memory(Func<int> getCurrentExecutionId)
{
	private Func<int> getCurrentExecutionId = getCurrentExecutionId;

	public void SetCurrentActionIdFunction(Func<int> getCurrentExecutionId)
	{
		this.getCurrentExecutionId = getCurrentExecutionId;
	}

	private readonly TrackedInputMemory trackedInputMemory = new TrackedInputMemory(getCurrentExecutionId);
	public IReadOnlyDictionary<int, Dictionary<string, string>> TrackedSnapshot()
		=> trackedInputMemory.TrackedInputsPerExecution();

	public T StoreTracked<T>(string key, Func<T> factory)
		=> trackedInputMemory.Store(key, factory);

	public T StoreStashed<T>(string key, Func<T> factory)
		=> trackedInputMemory.StoreWithoutReporting(key, factory);

	public void ResetRunScopedInputs()
		=> trackedInputMemory.Reset();



	private readonly Dictionary<int, Access> memoryPerExecution = [];
	public Access For(int executionId)
	{
		if (!memoryPerExecution.ContainsKey(executionId))
			memoryPerExecution[executionId] = new Access();
		return memoryPerExecution[executionId];
	}
	public Access ForThisExecution() => For(getCurrentExecutionId());



	private readonly Dictionary<int, Dictionary<string, string>> tracesPerExecution = [];
	public Dictionary<string, string> TracesFor(int executionId)
	{
		if (!tracesPerExecution.ContainsKey(executionId))
			tracesPerExecution[executionId] = [];
		return tracesPerExecution[executionId];
	}
	public Dictionary<string, string> TracesForThisExecution() => TracesFor(getCurrentExecutionId());


	private readonly Dictionary<int, Dictionary<string, List<string>>> diagnosisPerExecution = [];
	public Dictionary<string, List<string>> DiagnosisFor(int executionId)
	{
		if (!diagnosisPerExecution.ContainsKey(executionId))
			diagnosisPerExecution[executionId] = [];
		return diagnosisPerExecution[executionId];
	}
	public Dictionary<string, List<string>> DiagnosisForThisExecution() => DiagnosisFor(getCurrentExecutionId());



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
}
