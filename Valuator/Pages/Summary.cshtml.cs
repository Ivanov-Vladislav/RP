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
            _logger.LogDebug(id);
            
            string similarityKey = Constants.SimilarityKeyPrefix + id;
            Similarity = Convert.ToDouble(_storage.Load(similarityKey));
 
            string rankKey = Constants.RankKeyPrefix + id; 

            for (int retryCount = 0; retryCount < 20; retryCount++)
            {
                Thread.Sleep(100);
                if (_storage.IsKeyExist(rankKey))
                {
                    Rank = Convert.ToDouble(_storage.Load(rankKey));
                    return;
                }
            }

            _logger.LogWarning("RankKey {rankKey} doesn't exists", rankKey);

        }
    }
}
