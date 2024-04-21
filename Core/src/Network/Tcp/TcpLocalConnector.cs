namespace Markwardt;

[Factory<TcpLocalConnector>]
public delegate ValueTask<IConnector<ReadOnlyMemory<byte>>> TcpLocalConnectorFactory(int port);

public record TcpLocalConnector(int Port) : TcpConnector(new IpAddress("localhost", Port));