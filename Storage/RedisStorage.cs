using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Storage
{
    public class RedisStorage : IStorage
    {
        private readonly ILogger<RedisStorage> _logger;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly RedisKey _textIdentifiersKey = "textIdentifiers";
        private readonly string _host = "localhost";
        private Dictionary<string, IConnectionMultiplexer> _shartsConnectionMultiplexers;
 
        public RedisStorage(ILogger<RedisStorage> logger)
        {
            _logger = logger;
            _connectionMultiplexer = ConnectionMultiplexer.Connect(_host);
            _shartsConnectionMultiplexers = new Dictionary<string, IConnectionMultiplexer>()
            {
                { Constants.RusShardKey, ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_RUS", EnvironmentVariableTarget.User)) },
                { Constants.EuShardKey, ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_EU", EnvironmentVariableTarget.User)) },
                { Constants.OtherShardKey, ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_OTHER", EnvironmentVariableTarget.User)) }
            };
        }

        public void StoreShardKey(string id, string shardKey)
        {
            IDatabase db = _connectionMultiplexer.GetDatabase();
            db.StringSet(id, shardKey);
        }

        public void Store(string shardKey, string key, string value)
        {
            IDatabase db = GetConnectionMultiplexer(shardKey).GetDatabase();
            db.StringSet(key, value);
        }

        public void StoreToSet(string setIdentifier, string shardKey, string value)
        {
            IDatabase db = GetConnectionMultiplexer(shardKey).GetDatabase();
            db.SetAdd(setIdentifier, value);
        }

        public string Load(string shardKey, string key)
        {
            IDatabase db = GetConnectionMultiplexer(shardKey).GetDatabase();       
            return db.StringGet(key); 
        }

        public bool IsValueExistInSet(string setIdentifier, string shardKey, string value)
        {
            IDatabase db = GetConnectionMultiplexer(shardKey).GetDatabase();   
            return db.SetContains(setIdentifier, value);
        }

        public bool IsKeyExist(string shardKey, string key)
        {
            IDatabase db = GetConnectionMultiplexer(shardKey).GetDatabase();       
            return db.KeyExists(key);
        }

        public string GetShardKey(string id)
        {
            IDatabase db = _connectionMultiplexer.GetDatabase();     
            return db.StringGet(id);
        }

        private IConnectionMultiplexer GetConnectionMultiplexer(string shardKey)
        {
            IConnectionMultiplexer connectionMultiplexer;
            if (_shartsConnectionMultiplexers.TryGetValue(shardKey, out connectionMultiplexer))
            {
                IDatabase db = connectionMultiplexer.GetDatabase();
                return connectionMultiplexer;
            }

            _logger.LogWarning("Shard key {shardKey} doesn't exists", shardKey);
            return _connectionMultiplexer;
        }
    }
}