using System.Collections.Generic;

namespace Storage
{
    public interface IStorage
    {
        void StoreShardKey(string id, string shardKey);
        void Store(string shardKey, string key, string value);
        void StoreToSet(string setIdentifier, string shardKey, string value);
        string Load(string shardKey, string key);  
        bool IsValueExistInSet(string setIdentifier, string shardKey, string value);
        bool IsKeyExist(string shardKey, string key);
        string GetShardKey(string id);
    }
}
