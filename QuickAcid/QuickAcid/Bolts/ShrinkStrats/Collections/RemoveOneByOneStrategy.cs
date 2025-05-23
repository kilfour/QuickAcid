
using System.Collections;

namespace QuickAcid.Bolts.ShrinkStrats;

public class RemoveOneByOneStrategy : ICollectionShrinkStrategy
{
    public T Shrink<T>(QAcidState state, string key, T value, List<string> shrinkValues)
    {
        var theList = CloneAsOriginalTypeList(value);
        int index = 0;
        while (index < theList.Count)
        {
            var ix = index;
            var before = theList[ix];
            //-------------------------------------------------------
            // First try removing the element  
            //-------------------------------------------------------
            theList.RemoveAt(ix);
            if (state.ShrinkRunReturnTrueIfFailed(key, theList!))
            {
                shrinkValues.Add("_");
                continue;
            }
            shrinkValues.Add(QuickAcidStringify.Default()(before!));
            theList.Insert(ix, before);
            index++;
        }
        return (T)theList;
    }

    public static IList CloneAsOriginalTypeList(object value)
    {
        if (value is not IEnumerable enumerable)
            throw new ArgumentException("Value is not an IEnumerable", nameof(value));

        var valueType = value.GetType();
        Type elementType;

        if (valueType.IsArray)
        {
            elementType = valueType.GetElementType(); // ✅ Correct for arrays
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
