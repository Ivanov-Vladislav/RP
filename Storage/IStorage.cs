using System.Collections.Generic;

namespace Storage
{
    public interface IStorage
    {
        void Store(string key, string value);
        void StoreTextKey(string key);
        List<string> GetTextKeys();
        string Load(string key);  
        bool IsKeyExist(string key);
    }
}
