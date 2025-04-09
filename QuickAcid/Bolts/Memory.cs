using System.Text;
using QuickAcid.MonadiXEtAl;
using QuickAcid.Reporting;
using QuickMGenerate;

namespace QuickAcid.Bolts;

public class Memory
{
	private readonly Func<int> getCurrentActionId;

	public Access AlwaysReportedInputsPerRun { get; set; }
	private Dictionary<int, Dictionary<string, string>> AlwaysReportedInputValuePerExecution { get; set; }

	private Dictionary<int, Access> MemoryPerExecution { get; set; }

	public Memory(Func<int> getCurrentActionId)
	{
		this.getCurrentActionId = getCurrentActionId;
		AlwaysReportedInputsPerRun = new Access() { ActionKey = "AlwaysReported Inputs" };
		AlwaysReportedInputValuePerExecution = [];
		MemoryPerExecution = [];
	}

	public void AddExecutionToReport(int executionNumber, QAcidReport report, Exception exception)
	{
		if (AlwaysReportedInputValuePerExecution.TryGetValue(executionNumber, out Dictionary<string, string>? values))
		{
			values.ForEach(pair =>
				report.AddEntry(new ReportAlwaysReportedInputEntry(pair.Key) { Value = pair.Value })
			);
		}
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
		AlwaysReportedInputsPerRun = new Access() { ActionKey = "Once Only Inputs" };
		// -------------------------------------------------------------------------------
		// Let's really think if you want to do something like that below.
		// You've tried it before, ... more than once, it never worked and broke stuff.
		// Why do you think it will solve your problems now ?
		// --
		AlwaysReportedInputValuePerExecution = new Dictionary<int, Dictionary<string, string>>();
		// -------------------------------------------------------------------------------
	}

	public void AddAlwaysReportedInputValueForCurrentRun(string key, string value)
	{
		if (!AlwaysReportedInputValuePerExecution.ContainsKey(getCurrentActionId()))
			AlwaysReportedInputValuePerExecution[getCurrentActionId()] = [];
		//if (!AlwaysReportedInputValuePerAction[getCurrentActionId()].ContainsKey(key))
		AlwaysReportedInputValuePerExecution[getCurrentActionId()][key] = value;
	}

	public string ToDiagnosticString()
	{
		var sb = new StringBuilder();
		sb.AppendLine("=== Memory Dump ===");

		sb.AppendLine("--- OnceOnlyInputsPerRun ---");
		foreach (var kvp in AlwaysReportedInputsPerRun.GetAll())
		{
			sb.AppendLine($"{kvp.Key} = {kvp.Value.ToDiagnosticString()}");
		}

		sb.AppendLine("--- MemoryPerAction ---");
		foreach (var action in MemoryPerExecution)
		{
			sb.AppendLine($"Action {action.Key}:");
			foreach (var kvp in action.Value.GetAll())
			{
				sb.AppendLine($"  {kvp.Key} = {kvp.Value.ToDiagnosticString()}");
			}
		}

		return sb.ToString();
	}

	public T GetForFluentInterface<T>(string key)
	{
		return AlwaysReportedInputsPerRun.GetMaybe<T>(key)
			.OrElse(ForThisAction().GetMaybe<T>(key))
			.Match(
				some: x => x,
				none: () => throw new ThisNotesOnYou($"You're singing in the wrong key. '{key}' wasn't found in AlwaysReported(...) or Fuzzed(...).")
			);
	}
}
