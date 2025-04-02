namespace QuickAcid
{
	public class Memory
	{
		private readonly QAcidState state;

		//private readonly Func<Int> getC

		private Dictionary<int, Dictionary<object, object>> MemoryPerRun { get; set; }

		public Memory(QAcidState state)
		{
			this.state = state;
			MemoryPerRun = new Dictionary<int, Dictionary<object, object>>();
		}

		private Dictionary<object, object> GetThisRunsMemory()
		{
			if (!MemoryPerRun.ContainsKey(state.RunNumber))
				MemoryPerRun[state.RunNumber] = new Dictionary<object, object>();
			return MemoryPerRun[state.RunNumber];
		}

		public T Get<T>(object key, T newValue)
		{
			var dictionary = GetThisRunsMemory();
			if (!dictionary.ContainsKey(key))
				dictionary[key] = newValue;
			return (T)dictionary[key];
		}

		public T Get<T>(object key)
		{
			return (T)GetThisRunsMemory()[key];
		}

		public void Set<T>(object key, T value)
		{
			GetThisRunsMemory()[key] = value;
		}

		public bool ContainsKey(object key)
		{
			return GetThisRunsMemory().ContainsKey(key);
		}

		private Dictionary<int, HashSet<string>> ShrinkableKeysPerRun { get; set; } = new();
	}
}