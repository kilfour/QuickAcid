using System.Reflection;
using QuickAcid;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid
{
    public class Shrink
    {
        private static readonly Dictionary<Type, object[]> PrimitiveValues =
            new Dictionary<Type, object[]>
            {
                {typeof(int), new object[] { -1, 0, 1 }},
                {typeof(string), new object[] { null, "", new string('-', 256), new string('-', 1024) }},
            };

        public static void Input<T>(QAcidState state, object key, T value)
        {
            var shrunk = "Busy";
            state.Shrunk.Set(key, shrunk);
            if (typeof(IEnumerable<int>).IsAssignableFrom(typeof(T)))
            {
                shrunk = ShrinkIEnumerable(state, key, value);
                state.Shrunk.Set(key, shrunk);
                return;
            }
            var primitiveKey = PrimitiveValues.Keys.FirstOrDefault(k => k.IsAssignableFrom(typeof(T)));
            if (primitiveKey != null)
            {
                shrunk = ShrinkPrimitive(state, key, value, PrimitiveValues[primitiveKey]);
                state.Shrunk.Set(key, shrunk);
                return;
            }

            if (typeof(T).IsClass)
            {
                shrunk = HandleProperties(state, key, value);

                if (shrunk == "Irrelevant")
                {
                    var oldValues = new Dictionary<string, object>();
                    foreach (var propertyInfo in value.GetType().GetProperties(MyBinding.Flags))
                    {
                        oldValues[propertyInfo.Name] = propertyInfo.GetValue(value);
                    }

                    foreach (var set in GetPowerSet(value.GetType().GetProperties(MyBinding.Flags).ToList()))
                    {
                        if (shrunk != "Irrelevant")
                            break;

                        foreach (var propertyInfo in set)
                        {
                            SetPropertyValue(propertyInfo, value, null);
                        }

                        if (!state.ShrinkRun(key, value))
                        {
                            shrunk = string.Join(", ", set.Select(x => $"{x.Name} : {oldValues[x.Name]}"));
                        }

                        foreach (var propertyInfo in set)
                        {
                            SetPropertyValue(propertyInfo, value, oldValues[propertyInfo.Name]);
                        }
                    }
                }
            }

            state.Shrunk.Set(key, shrunk);
        }

        public static IEnumerable<IEnumerable<T>> GetPowerSet<T>(List<T> list)
        {
            return from m in Enumerable.Range(0, 1 << list.Count)
                   select
                       from i in Enumerable.Range(0, list.Count)
                       where (m & 1 << i) != 0
                       select list[i];
        }

        private static string HandleProperties<T>(QAcidState state, object key, T value)
        {
            var list = new List<string>();
            foreach (var propertyInfo in value.GetType().GetProperties(MyBinding.Flags))
            {
                var prop = HandleProperty(state, key, value, propertyInfo);
                if (prop != "Irrelevant")
                {
                    list.Add(prop);
                }
            }
            if (list.Any())
                return string.Join(", ", list);
            return "Irrelevant";
        }

        private static string HandleProperty(QAcidState state, object key, object value, PropertyInfo propertyInfo)
        {
            var propertyValue = propertyInfo.GetValue(value);
            var primitiveKey = PrimitiveValues.Keys.FirstOrDefault(k => k.IsAssignableFrom(propertyInfo.PropertyType));
            if (primitiveKey != null)
            {
                foreach (var primitiveValue in PrimitiveValues[primitiveKey])
                {
                    SetPropertyValue(propertyInfo, value, primitiveValue);
                    if (!state.ShrinkRun(key, value))
                    {
                        SetPropertyValue(propertyInfo, value, propertyValue);
                        return $"{propertyInfo.Name} : {propertyValue}";
                    }
                }
                SetPropertyValue(propertyInfo, value, propertyValue);
            }
            return "Irrelevant";
        }

        private static void SetPropertyValue(PropertyInfo propertyInfo, object target, object value)
        {
            var prop = propertyInfo;
            if (!prop.CanWrite)
                prop = propertyInfo.DeclaringType.GetProperty(propertyInfo.Name);

            if (prop.CanWrite) // todo check this
                prop.SetValue(target, value, null);
        }

        private static string ShrinkIEnumerable<T>(QAcidState state, object key, T value)
        {
            var theList = ((IEnumerable<int>)value).ToList();
            int index = 0;
            while (index < theList.Count)
            {
                var ix = index;
                var before = theList[ix];
                var primitiveVals = new[] { -1, 0, 1 };
                var removed = false;
                foreach (var primitiveVal in primitiveVals.Where(p => !p.Equals(before)))
                {
                    theList[ix] = primitiveVal;
                    var shrinkstate = state.ShrinkRun(key, theList);
                    if (shrinkstate)
                    {
                        theList.RemoveAt(index);
                        removed = true;
                        break;
                    }
                }
                if (!removed)
                {
                    theList[ix] = before;
                    index++;
                }
            }
            return $"[ {string.Join(", ", theList.Select(v => v.ToString()))} ]";
        }

        private static string ShrinkPrimitive(QAcidState state, object key, object value, object[] primitiveVals)
        {
            if (primitiveVals.Contains(value))
            {
                var originalShrinkState = state.ShrinkRun(key, value);
                return primitiveVals.Where(x => !x.Equals(value))
                    .Select(primitiveVal => state.ShrinkRun(key, primitiveVal))
                    .Any(shrinkstate => shrinkstate == originalShrinkState)
                    ? "Irrelevant"
                    : value.ToString();
            }
            return
                primitiveVals
                    .Select(primitiveVal => state.ShrinkRun(key, primitiveVal))
                    .Any(shrinkstate => shrinkstate)
                    ? "Irrelevant"
                    : value.ToString();
        }

        public static IEnumerable<object> Input(object value)
        {
            if (value == null) yield break;

            var type = value.GetType();

            if (type == typeof(int))
            {
                yield return 0;
                yield return 1;
                yield return -1;
            }
            else if (typeof(IEnumerable<int>).IsAssignableFrom(type))
            {
                var list = ((IEnumerable<int>)value).ToList();
                for (int i = 0; i < list.Count; i++)
                    yield return list.Where((_, idx) => idx != i).ToList();
            }
            else if (type == typeof(string))
            {
                yield return "";
                yield return null;
            }

            // You can plug in your advanced logic here later
        }

        public static string Summarize(object value)
        {
            if (value == null) return "null";

            var type = value.GetType();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (!props.Any()) return value.ToString();

            var summaries = props
                .Select(p => (Name: p.Name, Value: p.GetValue(value)))
                .Where(p => p.Value != null)
                .Select(p => $"{p.Name} : {p.Value}");

            return string.Join(", ", summaries);
        }
    }
}
