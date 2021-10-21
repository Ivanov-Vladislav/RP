using Storage;
using Common;
using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Valuator.Pages
{
    public class SummaryModel : PageModel
    {
        private readonly ILogger<SummaryModel> _logger;
        private readonly IStorage _storage;

        public SummaryModel(ILogger<SummaryModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public double Rank { get; set; }
        public double Similarity { get; set; }

        public void OnGet(string id)
        {                    
            string shardKey = _storage.GetShardKey(id);           

            _logger.LogDebug("LOOKUP: {id}, {shardKey}", id, shardKey);

            string rankKey = Constants.RankKeyPrefix + id; 
            string similarityKey = Constants.SimilarityKeyPrefix + id;

            Similarity = Convert.ToDouble(_storage.Load(shardKey, similarityKey));
 
            for (int retryCount = 0; retryCount < 20; retryCount++)
            {
                if (_storage.IsKeyExist(shardKey, rankKey))
                {
                    Rank = Convert.ToDouble(_storage.Load(shardKey, rankKey));
                    return;
                }
                Thread.Sleep(100);
            }

            _logger.LogWarning("RankKey {rankKey} doesn't exists", rankKey);

        }
    }
}
