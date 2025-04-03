using QuickMGenerate;

namespace QuickAcid
{
	public class Memory
	{
		public class Access
		{
			public class DecoratedValue
			{
				public object? Value { get; set; }
				public bool IsIrrelevant { get; set; }
				public string? ReportingMessage { get; set; }
			}

			public string? ActionKey { get; set; }
			public Exception? LastException { get; set; }
			public bool IsIrrelevant { get; set; }
			private Dictionary<object, DecoratedValue> dictionary = [];

			public T GetOrAdd<T>(object key, Func<T> factory)
			{
				if (!dictionary.ContainsKey(key))
					dictionary[key] = new DecoratedValue { Value = factory()!, IsIrrelevant = false };
				return Get<T>(key);
			}

			public T Get<T>(object key)
			{
				return (T)dictionary[key].Value!;
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

			public void AddToReport(QAcidReport report)
			{
				foreach (var pair in GetAll())
				{
					if (pair.Value!.IsIrrelevant) continue;
					var value = string.IsNullOrEmpty(pair.Value.ReportingMessage) ? pair.Value.Value : pair.Value.ReportingMessage;
					report.AddEntry(new QAcidReportInputEntry(pair.Key) { Value = value });
				}
				report.AddEntry(new QAcidReportActEntry(ActionKey!) { Exception = LastException });
			}
		}
		private readonly Func<int> getCurrentActionId;

		public Access OnceOnlyInputsPerRun { get; set; }

		private Dictionary<int, Access> MemoryPerAction { get; set; }

		public Memory(Func<int> getCurrentActionId)
		{
			this.getCurrentActionId = getCurrentActionId;
			OnceOnlyInputsPerRun = new Access() { ActionKey = "Once Only Inputs" };
			MemoryPerAction = [];
		}

		public void AddActionToReport(int actionNumber, QAcidReport report)
		{
			if (MemoryPerAction.ContainsKey(actionNumber))
				MemoryPerAction[actionNumber].AddToReport(report);
		}

		public Access ForThisAction()
		{
			if (!MemoryPerAction.ContainsKey(getCurrentActionId()))
				MemoryPerAction[getCurrentActionId()] = new Access();
			return MemoryPerAction[getCurrentActionId()];
		}

		public void ResetAllRunInputs()
		{
			OnceOnlyInputsPerRun = new Access() { ActionKey = "Once Only Inputs" };
		}

		public void AddToReport(QAcidReport report)
		{
			MemoryPerAction.ForEach(a => a.Value.AddToReport(report));
		}
	}
}