using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Valuator
{
    public class RedisStorage : IStorage
    {
        private readonly ILogger<RedisStorage> _logger;
        private IConnectionMultiplexer _connection;
        
        public RedisStorage(ILogger<RedisStorage> logger)
        {
            _logger = logger;
            _connection = ConnectionMultiplexer.Connect("localhost");
        }
        public void Store(string key, string value)
        {
            var db = _connection.GetDatabase();
            db.StringSet(key, value);
        }
        public string Load(string key)
        {
            var db = _connection.GetDatabase();
            return db.StringGet(key);
        }
        public HashSet<string> GetKeysWithPrefix(string prefix)
        {
            var server = _connection.GetServer("localhost", 6379);
            HashSet<string> keys = new HashSet<string>();
            foreach (var key in server.Keys(pattern: prefix + "*"))
            {
                keys.Add(key);
            }
            return keys;
        }
    }
}
