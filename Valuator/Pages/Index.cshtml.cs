using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

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

        public IActionResult OnPost(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Redirect("/");
            }

            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();   
            
            string rankKey = "RANK-" + id;
            string rank = GetRank(text).ToString();

            _storage.Store(rankKey, rank);

            string similarityKey = "SIMILARITY-" + id;
            double similarity = GetSimilarity(text);

            _storage.Store(similarityKey, similarity.ToString());

            if (similarity == 0)
            {
                string textKey = "TEXT-" + id;
                _storage.Store(textKey, text);
                _storage.StoreTextKey(textKey);
            }

            return Redirect($"summary?id={id}");
        }

        private double GetRank(string text)
        {   
            int lettersCount = text.Count(char.IsLetter);

            return Math.Round(((text.Length - lettersCount) / (double)text.Length), 3);
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
