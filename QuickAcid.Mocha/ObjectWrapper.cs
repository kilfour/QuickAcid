using Jint;
using Jint.Native;
using Jint.Native.Object;

namespace QuickAcid.Mocha;

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