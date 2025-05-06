using Jint;
using Jint.Runtime;
using Jint.Native;
using Jint.Native.Object;

namespace QuickAcid.Mocha;

public static class From
{
    public static ModulesWrappper Path(string path)
    {
        return new ModulesWrappper(path);
    }
}

public class ModulesWrappper
{
    private readonly Engine engine;

    public ModulesWrappper(string path)
    {
        engine = new Engine(opts => { opts.EnableModules(path); });
    }
    public ModuleWrappper AndFile(string fileName)
    {
        return new ModuleWrappper(engine, fileName);
    }
}

public class ModuleWrappper
{
    private readonly Engine engine;
    private readonly ObjectInstance accountModule;

    public ModuleWrappper(Engine engine, string fileName)
    {
        this.engine = engine;
        accountModule = engine.Modules.Import(fileName);
    }

    public ObjectWrapper Construct(string className)
    {
        var accountCtor = accountModule.Get(className);
        var anObject = engine.Construct(accountCtor, Arguments.Empty);
        return new ObjectWrapper(anObject);
    }
}

public class ObjectWrapper
{
    private readonly ObjectInstance anObject;

    public ObjectWrapper(ObjectInstance anObject)
    {
        this.anObject = anObject;
    }

    public JsValue Call(string methodName, params JsValue[] args)
    {
        return anObject.Get(methodName).Call(anObject, args);
    }
}