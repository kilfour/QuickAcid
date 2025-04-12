using QuickAcid.MonadiXEtAl;
using QuickAcid.Reporting;

namespace QuickAcid.Bolts;

public class Memory
{
	private readonly Func<int> getCurrentActionId;
	private readonly AlwaysReportedInputMemory alwaysReportedInputMemory;
	private readonly Dictionary<int, Access> memoryPerExecution = [];

	public Memory(Func<int> getCurrentActionId)
	{
		this.getCurrentActionId = getCurrentActionId;
		this.alwaysReportedInputMemory = new AlwaysReportedInputMemory(getCurrentActionId);
	}

	public T StoreAlwaysReported<T>(string key, Func<T> factory, Func<T, string> stringify)
		=> alwaysReportedInputMemory.Store(key, factory, stringify);

	public Maybe<T> RetrieveAlwaysReported<T>(string key)
		=> alwaysReportedInputMemory.Retrieve<T>(key);

	public void ResetAllRunInputs()
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

	public Access ForThisAction()
	{
		var actionId = getCurrentActionId();
		if (!memoryPerExecution.ContainsKey(actionId))
			memoryPerExecution[actionId] = new Access();
		return memoryPerExecution[actionId];
	}

	public Access ForLastAction() // used by codegen
	{
		return memoryPerExecution.Last().Value;
	}

	public bool Has(int actionId) // used by codegen
		=> memoryPerExecution.ContainsKey(actionId);
}
