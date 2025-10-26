using StackExchange.Redis;

namespace DroneSim.Infrastructure.Redis;

public class RedisConnectionFactory(string configuration)
{
    private readonly Lazy<ConnectionMultiplexer> _connection = 
            new Lazy<ConnectionMultiplexer>(() =>
                ConnectionMultiplexer.Connect(configuration)
            );
    public IConnectionMultiplexer GetConnection() => _connection.Value;
}
