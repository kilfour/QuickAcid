using QuickMGenerate;

namespace QuickAcid
{
	public class Memory
	{
		public class Access2
		{
			public class DecoratedValue
			{
				public object? Value { get; set; }
				public bool IsIrrelevant { get; set; }
			}

			public string? ActionKey { get; set; }
			public Exception? LastException { get; set; }

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

			public bool ContainsKey(object key)
			{
				return dictionary.ContainsKey(key);
			}

			public Dictionary<string, object?> GetAll()
			{
				return dictionary
					.Where(kvp => kvp.Key is string)
					.ToDictionary(kvp => (string)kvp.Key, kvp => kvp.Value.Value);
			}

			public void AddToReport(QAcidReport report)
			{
				foreach (var pair in GetAll())
				{
					report.AddEntry(new QAcidReportInputEntry(pair.Key)
					{
						Value = pair.Value
					});
				}
				report.AddEntry(new QAcidReportActEntry(ActionKey!) { Exception = LastException });
			}
		}

		public class Access
		{
			public string? ActionKey { get; set; }
			public Exception? LastException { get; set; }

			private Dictionary<object, object> dictionary = [];

			public T GetOrAdd<T>(object key, Func<T> factory)
			{
				if (!dictionary.ContainsKey(key))
					dictionary[key] = factory()!;
				return (T)dictionary[key];
			}

			public T Get<T>(object key)
			{
				return (T)dictionary[key];
			}

			public void Set<T>(object key, T value)
			{
				dictionary[key] = value!;
			}

			public bool ContainsKey(object key)
			{
				return dictionary.ContainsKey(key);
			}
			public Dictionary<string, object> GetAll()
			{
				return dictionary
					.Where(kvp => kvp.Key is string)
					.ToDictionary(kvp => (string)kvp.Key, kvp => kvp.Value);
			}

			public void AddToReport(QAcidReport report)
			{
				foreach (var pair in GetAll())
				{
					report.AddEntry(new QAcidReportInputEntry(pair.Key)
					{
						Value = pair.Value
					});
				}
				report.AddEntry(new QAcidReportActEntry(ActionKey!) { Exception = LastException });
			}
		}

		private readonly Func<int> getCurrentActionId;

		public Access OnceOnlyInputsPerRun { get; set; }

		private Dictionary<int, Access2> MemoryPerAction { get; set; }

		public Memory(Func<int> getCurrentActionId)
		{
			this.getCurrentActionId = getCurrentActionId;
			OnceOnlyInputsPerRun = new Access() { ActionKey = "Once Only Inputs" };
			MemoryPerAction = [];
		}

		public Access2 ForThisAction()
		{
			if (!MemoryPerAction.ContainsKey(getCurrentActionId()))
				MemoryPerAction[getCurrentActionId()] = new Access2();
			return MemoryPerAction[getCurrentActionId()];
		}

		public void ResetAllRunInputs()
		{
			OnceOnlyInputsPerRun = new Access() { ActionKey = "Once Only Inputs" };
		}

		public Dictionary<string, object> GetAll()
		{
			return MemoryPerAction
				.SelectMany(kvp => kvp.Value.GetAll())
				.GroupBy(kvp => kvp.Key)
				.ToDictionary(g => g.Key, g => g.First().Value);
		}

		public void AddToReport(QAcidReport report)
		{
			MemoryPerAction.ForEach(a => a.Value.AddToReport(report));
		}
	}
}