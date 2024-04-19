namespace Markwardt;

[Factory<LiteNetLocalConnector>]
public delegate ValueTask<IConnector<ReadOnlyMemory<byte>>> LiteNetLocalConnectorFactory(int port);

public record LiteNetLocalConnector(int Port) : LiteNetConnector(new IpAddress("localhost", Port));