
using System.Collections;

namespace QuickAcid.Shrinking.Collections
{
    public static class CloneList
    {
        public static IList AsOriginalType(object value)
        {
            if (value is not IEnumerable enumerable)
                throw new ArgumentException("Value is not an IEnumerable", nameof(value));

            var valueType = value.GetType();
            Type elementType;

            if (valueType.IsArray)
            {
                elementType = valueType.GetElementType()!; // ✅ Correct for arrays
            }
            else if (valueType.IsGenericType)
            {
                elementType = valueType.GetGenericArguments().First();
            }
            else
            {
                elementType = typeof(object);
            }

            var typedListType = typeof(List<>).MakeGenericType(elementType);
            var typedList = (IList)Activator.CreateInstance(typedListType)!;

            foreach (var item in enumerable)
            {
                typedList.Add(item);
            }

            return typedList;
        }
    }
}