namespace QuickAcid.Examples.BroadcasterExample.SimpleModel;

public interface IClientProxyFactory
{
    IClientProxy CreateClientProxyForCurrentContext(string s);
}

public class TestClientProxyFactory : IClientProxyFactory
{
    private readonly List<TestClientProxy> _clients = new();

    public IReadOnlyList<TestClientProxy> CreatedClients => _clients;

    public IClientProxy CreateClientProxyForCurrentContext(string s)
    {
        var client = new TestClientProxy();
        _clients.Add(client);
        return client;
    }
}
