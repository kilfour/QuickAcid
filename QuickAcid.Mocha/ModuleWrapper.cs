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
}
