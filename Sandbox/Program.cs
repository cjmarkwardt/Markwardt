using Markwardt;

ServiceContainer services = new(new DefaultHandler());

UnicodeNetworkConverter converter = new();

await Task.WhenAll
(
    NetworkTest.Host((await (await services.RequireDefault<SteamHosterFactory>())()).ConvertMessages(converter)).AsTask(),
    NetworkTest.Connect((await (await services.RequireDefault<SteamLocalConnectorFactory>())()).ConvertMessages(converter)).AsTask()
);