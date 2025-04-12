using System.Text;
using QuickAcid.MonadiXEtAl;
using QuickAcid.Reporting;
using QuickMGenerate;

namespace QuickAcid.Bolts;

public class Memory
{
	private readonly Func<int> getCurrentActionId;

	private AlwaysReportedInputMemory alwaysReportedInputMemory;

	private Dictionary<int, Access> MemoryPerExecution { get; set; }

	public Memory(Func<int> getCurrentActionId)
	{
		this.getCurrentActionId = getCurrentActionId;
		alwaysReportedInputMemory = new AlwaysReportedInputMemory(getCurrentActionId);
		MemoryPerExecution = [];
	}
	public T StoreAlwaysReported<T>(string key, Func<T> factory, Func<T, string> stringify)
	{
		return alwaysReportedInputMemory.Store(key, factory, stringify);
	}

	public Maybe<T> RetrieveAlwaysReported<T>(string key)
	{
		return alwaysReportedInputMemory.Retrieve<T>(key);
	}
	public void AddExecutionToReport(int executionNumber, QAcidReport report, Exception exception)
	{
		alwaysReportedInputMemory.AddToReport(report, executionNumber);
		// if (alwaysReportedInputMemory.AddToReport(executionNumber, out Dictionary<string, string>? values))
		// {
		// 	values.ForEach(pair =>
		// 		report.AddEntry(new ReportAlwaysReportedInputEntry(pair.Key) { Value = pair.Value })
		// 	);
		// }
		if (MemoryPerExecution.ContainsKey(executionNumber))
			MemoryPerExecution[executionNumber].AddToReport(report, exception);
	}

	public bool Has(int actionId) // used by codegen, leaving a bit of a mess all over
	{
		return MemoryPerExecution.ContainsKey(actionId);
	}

	public Access For(int actionId) // used by codegen, assumes it exists, ... careful now
	{
		// might go ploef
		return MemoryPerExecution[actionId];
	}

	public Access ForLastAction() // used by codegen
	{
		return MemoryPerExecution.Last().Value;
	}

	public Access ForThisAction()
	{
		if (!MemoryPerExecution.ContainsKey(getCurrentActionId()))
			MemoryPerExecution[getCurrentActionId()] = new Access();
		return MemoryPerExecution[getCurrentActionId()];
	}

	public void ResetAllRunInputs()
	{
		alwaysReportedInputMemory.Reset();
	}

	// public void AddAlwaysReportedInputValueForCurrentRun(string key, string value)
	// {
	// 	if (!AlwaysReportedInputValuePerExecution.ContainsKey(getCurrentActionId()))
	// 		AlwaysReportedInputValuePerExecution[getCurrentActionId()] = [];
	// 	AlwaysReportedInputValuePerExecution[getCurrentActionId()][key] = value;
	// }


	public T GetForFluentInterface<T>(string key)
	{
		return alwaysReportedInputMemory.Retrieve<T>(key)
			.OrElse(ForThisAction().GetMaybe<T>(key))
			.Match(
				some: x => x,
				none: () => throw new ThisNotesOnYou($"You're singing in the wrong key. '{key}' wasn't found in AlwaysReported(...) or Fuzzed(...).")
			);
	}
}
