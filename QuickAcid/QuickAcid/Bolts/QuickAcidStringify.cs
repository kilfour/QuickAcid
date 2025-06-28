using System.Collections;

namespace QuickAcid.Bolts;

public static class QuickAcidStringify
{
    public static Func<T, string> Default<T>() => obj => Default()(obj!);
    public static Func<object, string> Default() => obj =>
    {
        if (obj is null)
            return "<null>";

        if (obj is string str)
            return $"\"{str}\"";

        var actualType = obj.GetType(); // TODO DOC THIS WITH TEST
        if (actualType.GetMethod("ToString", Type.EmptyTypes)?.DeclaringType != typeof(object))
            return obj.ToString()!;

        if (obj is IEnumerable enumerable && obj is not IDictionary)
        {
            const int MaxItems = 10;
            var items = new List<string>();
            int count = 0;

            foreach (var item in enumerable)
            {
                if (count++ >= MaxItems)
                {
                    items.Add("...");
                    break;
                }

                items.Add(item == null ? "<null>" : item.ToString()!);
            }

            return "[ " + string.Join(", ", items) + " ]";
        }

        var type = obj.GetType();

        if (type.IsPrimitive || obj is decimal or DateTime or Guid or TimeSpan)
            return obj.ToString()!;

        var props = type.GetProperties()
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            .ToArray();

        if (props.Length == 0)
            return obj.ToString()!;

        var content = string.Join(", ",
            props.Select(p =>
            {
                var val = p.GetValue(obj);
                var valStr = val == null ? "<null>" : val.ToString();
                return $"{p.Name} = {valStr}";
            }));

        return $"{type.Name} {{ {content} }}";
    };
}