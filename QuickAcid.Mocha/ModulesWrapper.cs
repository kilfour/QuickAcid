using Jint;

namespace QuickAcid.Mocha;

public class ModulesWrapper
{
    private readonly Engine engine;

    public ModulesWrapper(string path)
    {
        engine = new Engine(opts => { opts.EnableModules(path); });
    }

    public ModuleWrapper AndFile(string fileName)
    {
        return new ModuleWrapper(engine, fileName);
    }
}
