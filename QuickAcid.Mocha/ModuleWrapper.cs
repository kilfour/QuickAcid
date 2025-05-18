using Jint;
using Jint.Runtime;
using Jint.Native.Object;
using Jint.Native;

namespace QuickAcid.Mocha;

public class ModuleWrapper
{
    private readonly Engine engine;
    private readonly ObjectInstance module;

    public ModuleWrapper(Engine engine, string fileName)
    {
        this.engine = engine;
        module = engine.Modules.Import(fileName);
    }

    public ObjectWrapper Construct(string className)
    {
        var ctor = module.Get(className);
        var anObject = engine.Construct(ctor, Arguments.Empty);
        return new ObjectWrapper(anObject);
    }

    public JsValue Call(string methodName, params JsValue[] args)
    {
        return module.Get(methodName).Call(args);
    }

    public JsValue CreateJsArray(int[] values)
    {
        var jsArray = engine.Intrinsics.Array.Construct(Arguments.Empty);
        for (uint i = 0; i < values.Length; i++)
        {
            jsArray.Set(i, JsNumber.Create(values[i]), throwOnError: false);
        }
        return jsArray;
    }

    // public static JsValue CreateJsArray<T>(Engine engine, T[] items, Func<T, JsValue> toJsValue)
    // {
    //     var jsArray = engine.Intrinsics.Array.Construct(Arguments.Empty);
    //     for (uint i = 0; i < items.Length; i++)
    //     {
    //         jsArray.Set(i, toJsValue(items[i]), throwOnError: false);
    //     }
    //     return jsArray;
    // }
}

