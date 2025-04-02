namespace QuickAcid
{
	public class Memory
	{
		public class Access
		{
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
		}

		private readonly Func<int> getCurrentActionId;

		public Access OnceOnlyInputsPerRun { get; set; }

		private Dictionary<int, Access> MemoryPerAction { get; set; }

		public Memory(Func<int> getCurrentActionId)
		{
			this.getCurrentActionId = getCurrentActionId;
			OnceOnlyInputsPerRun = new Access();
			MemoryPerAction = [];
		}

		public Access ForThisAction()
		{
			if (!MemoryPerAction.ContainsKey(getCurrentActionId()))
				MemoryPerAction[getCurrentActionId()] = new Access();
			return MemoryPerAction[getCurrentActionId()];
		}

		public void ResetAllRunInputs()
		{
			OnceOnlyInputsPerRun = new Access();
		}

		public Dictionary<string, object> GetAll()
		{
			return MemoryPerAction
				.SelectMany(kvp => kvp.Value.GetAll())
				.GroupBy(kvp => kvp.Key)
				.ToDictionary(g => g.Key, g => g.First().Value);
		}
	}
}