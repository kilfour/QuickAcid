using System.Text;
using QuickAcid.MonadiXEtAl;
using QuickAcid.Reporting;
using QuickMGenerate;

namespace QuickAcid.Nuts;

public class Memory
{
	public class Access
	{
		public class DecoratedValue
		{
			public object? Value { get; set; }
			public bool IsIrrelevant { get; set; }
			public string? ReportingMessage { get; set; }
			public Func<object, string>? Stringify { get; set; }

			public string ToDiagnosticString()
			{
				var sb = new StringBuilder();
				if (Value == null)
					sb.Append("null");
				else
					sb.Append(Stringify == null ? Value : Stringify(Value));
				if (IsIrrelevant)
					sb.Append(" : Irrelevant");
				if (ReportingMessage != null)
					sb.AppendFormat(", {0}", ReportingMessage);
				return sb.ToString();
			}
		}

		public string? ActionKey { get; set; }

		public Exception? LastException { get; set; }
		public bool IsIrrelevant { get; set; }
		private Dictionary<object, DecoratedValue> dictionary = [];

		public T GetOrAdd<T>(object key, Func<T> factory, Func<T, string> stringify)
		{
			if (!dictionary.ContainsKey(key))
				dictionary[key] = new DecoratedValue { Value = factory()!, IsIrrelevant = false, Stringify = obj => stringify((T)obj) };
			return Get<T>(key);
		}

		public T Get<T>(object key)
		{
			return (T)dictionary[key].Value!;
		}

		public Maybe<T> GetMaybe<T>(object key)
		{
			return dictionary.TryGetValue(key, out var value)
				? Maybe<T>.Some((T)value.Value!)
				: Maybe<T>.None;
		}

		public void Set<T>(object key, T value)
		{
			if (!dictionary.ContainsKey(key))
				dictionary[key] = new DecoratedValue { Value = value!, IsIrrelevant = false };
			else
				dictionary[key].Value = value!;
		}

		public void MarkAsIrrelevant<T>(object key)
		{
			dictionary[key].IsIrrelevant = true;
		}

		public void AddReportingMessage<T>(object key, string message)
		{
			dictionary[key].ReportingMessage = message;
		}

		public bool ContainsKey(object key)
		{
			return dictionary.ContainsKey(key);
		}

		public Dictionary<string, DecoratedValue> GetAll()
		{
			return dictionary
				.Where(kvp => kvp.Key is string)
				.ToDictionary(kvp => (string)kvp.Key, kvp => kvp.Value);
		}

		public void AddToReport(QAcidReport report, Exception exceptionFromState)
		{
			foreach (var pair in GetAll())
			{
				if (pair.Value!.IsIrrelevant) continue;
				var value = string.IsNullOrEmpty(pair.Value.ReportingMessage) ? pair.Value.Value : pair.Value.ReportingMessage;
				report.AddEntry(new ReportInputEntry(pair.Key) { Value = value });
			}
			bool isSameException = LastException?.ToString() == exceptionFromState?.ToString();

			report.AddEntry(
				new ReportActEntry(ActionKey!)
				{
					Exception = isSameException ? LastException : null
				});
		}
	}

	private readonly Func<int> getCurrentActionId;

	public Access AlwaysReportedInputsPerRun { get; set; }
	private Dictionary<int, Dictionary<string, string>> AlwaysReportedInputValuePerAction { get; set; }

	private Dictionary<int, Access> MemoryPerAction { get; set; }

	public Memory(Func<int> getCurrentActionId)
	{
		this.getCurrentActionId = getCurrentActionId;
		AlwaysReportedInputsPerRun = new Access() { ActionKey = "AlwaysReported Inputs" };
		AlwaysReportedInputValuePerAction = new Dictionary<int, Dictionary<string, string>>();
		MemoryPerAction = [];
	}

	public void AddActionToReport(int actionNumber, QAcidReport report, Exception exception)
	{
		if (AlwaysReportedInputValuePerAction.TryGetValue(actionNumber, out Dictionary<string, string>? values))
		{
			values.ForEach(pair =>
				report.AddEntry(new QAcidReportAlwaysReportedInputEntry(pair.Key) { Value = pair.Value })
			);
		}
		if (MemoryPerAction.ContainsKey(actionNumber))
			MemoryPerAction[actionNumber].AddToReport(report, exception);
	}

	public bool Has(int actionId) // used by codegen, leaving a bit of a mess all over
	{
		return MemoryPerAction.ContainsKey(actionId);
	}

	public Access For(int actionId) // used by codegen, assumes it exists, ... careful now
	{
		// might go ploef
		return MemoryPerAction[actionId];
	}

	public Access ForLastAction() // used by codegen
	{
		return MemoryPerAction.Last().Value;
	}

	public Access ForThisAction()
	{
		if (!MemoryPerAction.ContainsKey(getCurrentActionId()))
			MemoryPerAction[getCurrentActionId()] = new Access();
		return MemoryPerAction[getCurrentActionId()];
	}

	public void ResetAllRunInputs()
	{
		AlwaysReportedInputsPerRun = new Access() { ActionKey = "Once Only Inputs" };
		// -------------------------------------------------------------------------------
		// Let's really think if you want to do something like that below.
		// You've tried it before, ... more than once, it never worked and broke stuff.
		// Why do you think it will solve your problems now ?
		// --
		// AlwaysReportedInputValuePerAction = new Dictionary<int, Dictionary<string, string>>();
		// -------------------------------------------------------------------------------
	}

	public void AddAlwaysReportedInputValueForCurrentRun(string key, string value)
	{
		if (!AlwaysReportedInputValuePerAction.ContainsKey(getCurrentActionId()))
			AlwaysReportedInputValuePerAction[getCurrentActionId()] = [];
		if (!AlwaysReportedInputValuePerAction[getCurrentActionId()].ContainsKey(key))
			AlwaysReportedInputValuePerAction[getCurrentActionId()][key] = value;
	}

	public void AddToReport(QAcidReport report, Exception exception) // only used in test, bweurk
	{
		MemoryPerAction.ForEach(a => a.Value.AddToReport(report, exception));
	}

	public string ToDiagnosticString()
	{
		var sb = new StringBuilder();
		sb.AppendLine("=== Memory Dump ===");

		// sb.AppendLine("--- OnceOnlyInputsPerRun ---");
		// foreach (var kvp in OnceOnlyInputsPerRun.GetAll())
		// {
		// 	sb.AppendLine($"{kvp.Key} = {kvp.Value.ToDiagnosticString()}");
		// }

		// sb.AppendLine("--- MemoryPerAction ---");
		// foreach (var action in MemoryPerAction)
		// {
		// 	sb.AppendLine($"Action {action.Key}:");
		// 	foreach (var kvp in action.Value.GetAll())
		// 	{
		// 		sb.AppendLine($"  {kvp.Key} = {kvp.Value.ToDiagnosticString()}");
		// 	}
		// }

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
