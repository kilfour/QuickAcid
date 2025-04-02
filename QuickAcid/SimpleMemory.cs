namespace QuickAcid
{
    public class SimpleMemory
    {
        private Dictionary<object, object> Dictionary { get; set; }

        public SimpleMemory()
        {
            Dictionary = new Dictionary<object, object>();
        }

        public T Get<T>(object key, T newValue)
        {
            if (!Dictionary.ContainsKey(key))
                Dictionary[key] = newValue;
            return (T)Dictionary[key];
        }

        public T Get<T>(object key, Func<T> newValue)
        {
            if (!Dictionary.ContainsKey(key))
                Dictionary[key] = newValue();
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
        public Dictionary<string, object> GetAllShrinkableInputs()
        {
            return Dictionary
                .Where(kvp => kvp.Key is string) // assuming shrinkable input keys are strings (like "withdraw amount")
                .ToDictionary(
                    kvp => (string)kvp.Key,
                    kvp => kvp.Value
                );
        }

        public Dictionary<string, object> GetAll()
        {
            return Dictionary
                .Where(kvp => kvp.Key is string) // assuming shrinkable input keys are strings (like "withdraw amount")
                .ToDictionary(
                    kvp => (string)kvp.Key,
                    kvp => kvp.Value
                );
        }
        public void AddShrinkableInput<T>(object key, T value)
        {
            Dictionary[key] = value;
        }

    }
}