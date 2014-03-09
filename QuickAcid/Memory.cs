using System;
using System.Collections.Generic;

namespace QuickAcid
{
	public class Memory
	{
		private readonly QAcidState state;

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
			
			try
			{
				return (T)GetThisRunsMemory()[key];
			}
			catch (Exception)
			{
				//Console.WriteLine("Trying to get '{0}' for run '{1}'.", key, state.RunNumber);
				return default(T);
			}
			
		}

		public void Set<T>(object key, T value)
		{
			//Console.WriteLine("Wrote '{0}' to '{1}' for run '{2}'.", value, key, state.RunNumber);
			GetThisRunsMemory()[key] = value;
		}

		public bool ContainsKey(object key)
		{
			return GetThisRunsMemory().ContainsKey(key);
		}
	}

	public class GlobalMemory
	{
		private Dictionary<object, object> Dictionary { get; set; }

		public GlobalMemory()
		{
			Dictionary = new Dictionary<object, object>();
		}

		public T Get<T>(object key, T newValue)
		{
			if (!Dictionary.ContainsKey(key))
				Dictionary[key] = newValue;
			return (T)Dictionary[key];
		}

		public T Get<T>(object key)
		{
			return (T)Dictionary[key];
		}

		public void Set<T>(object key, T value)
		{
			Dictionary[key] = value;
		}
	}
}