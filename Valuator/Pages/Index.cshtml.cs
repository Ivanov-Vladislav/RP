using Common;
using Storage;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using NATS.Client;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;

        public IndexModel(ILogger<IndexModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPost(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Redirect("/");
            }

            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();   
            string textKey = Constants.TextKeyPrefix + id;

            string similarityKey = Constants.SimilarityKeyPrefix + id;
            double similarity = GetSimilarity(text);

            _storage.Store(similarityKey, similarity.ToString());

            _storage.Store(textKey, text);
            _storage.StoreTextKey(textKey);

            await CreateTaskForRankCalculator(id);

            SimilarityMessage similarityMessage = new SimilarityMessage(id, similarity);
            await SentMessageToEventLogger(similarityMessage);

            return Redirect($"summary?id={id}");
        }

        private async Task CreateTaskForRankCalculator(string id)
        {  
            using (IConnection c = new ConnectionFactory().CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(id);
                c.Publish("valuator.processing.rank", data);    

                await Task.Delay(1000);  
     
                c.Drain();
                c.Close();
            }
        }

        private async Task SentMessageToEventLogger(SimilarityMessage similarityMsg)
        {
            using (IConnection c = new ConnectionFactory().CreateConnection())
            {
                var data = JsonSerializer.Serialize(similarityMsg);
                c.Publish("valuator.logging.similarity", Encoding.UTF8.GetBytes(data));

                await Task.Delay(1000);

                c.Drain();
                c.Close();
            }
        }

        private double GetSimilarity(string text)
        {
            var keys = _storage.GetTextKeys();
            
            foreach (var key in keys)
            {
                if (_storage.Load(key) == text)
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}
