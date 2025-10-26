using StackExchange.Redis;

namespace DroneSim.Infrastructure.Redis;

public class RedisConnectionFactory(string configuration)
{
    private readonly Lazy<ConnectionMultiplexer> _connection = 
            new Lazy<ConnectionMultiplexer>(() =>
            {
                var options = ConfigurationOptions.Parse(configuration);
                options.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(options);
            });
    public IConnectionMultiplexer GetConnection() => _connection.Value;
}
