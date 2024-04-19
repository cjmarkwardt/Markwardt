namespace Markwardt;

public static class NetworkTest
{
    public static async ValueTask Host(IHoster<object> hoster)
    {
        Console.WriteLine("SERVER HOSTING");

        IServer<object> server = new Server<object>();
        Failable tryHost = await server.Host(hoster);
        if (tryHost.Exception is not null)
        {
            Console.WriteLine($"SERVER FAILED TO HOST ({tryHost.Exception.Message})");
            return;
        }

        server.Receives.Consume().Subscribe(async x =>
        {
            Console.WriteLine($"SERVER CONNECTED");
            await using IConnection<object> connection = x;
            connection.Disconnection.Subscribe(() => Console.WriteLine("SERVER DISCONNECTED"));
            connection.Receives.Consume().Subscribe(message => Console.WriteLine($"SERVER RECEIVED: {message}"));
            connection.Send("server message");
            await Task.Delay(5000);
            connection.Disconnect();

            await Task.Delay(-1);
        });

        await Task.Delay(-1);
    }

    public static async ValueTask Connect(IConnector<object> connector)
    {
        Console.WriteLine("CLIENT CONNECTING");

        Failable<IConnection<object>> tryConnect = await connector.Connect();
        if (tryConnect.Exception is not null)
        {
            Console.WriteLine($"CLIENT FAILED TO CONNECT ({tryConnect.Exception.Message})");
            return;
        }

        await using IConnection<object> connection = tryConnect.Result;
        Console.WriteLine($"CLIENT CONNECTED");
        connection.Disconnection.Subscribe(() => Console.WriteLine("CLIENT DISCONNECTED"));
        connection.Receives.Consume().Subscribe(message => Console.WriteLine($"CLIENT RECEIVED: {message}"));
        connection.Send("client message");

        await Task.Delay(-1);
    }
}