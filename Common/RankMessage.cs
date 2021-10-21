
using System;

namespace Common
{
    [Serializable]
    public struct RankMessage
    {
        public string Id { get; set; }
        public double Rank { get; set; }

        public RankMessage(string id, double rank ) 
        {
            Id = id;
            Rank = rank;
        }
    }
}