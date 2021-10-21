using NATS.Client;
using System;
using System.Text;
using System.Linq;
using Valuator;
using Microsoft.Extensions.Logging;

namespace RankCalculator
{
    public class RankCalculator
    {
        private readonly IConnection _connection;
        private readonly IAsyncSubscription _subscription;
        private readonly ILogger<RankCalculator> _logger;
        
        public RankCalculator(ILogger<RankCalculator> logger, IStorage storage)
        {
            _logger = logger;
            _connection = new ConnectionFactory().CreateConnection();

            _subscription = _connection.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                string textKey = Constants.TextKeyPrefix + id;

                if (!storage.IsKeyExist(textKey))
                {
                    logger.LogWarning("Text key {textKey} doesn't exists", textKey);
                    return;
                }

                string text = storage.Load(textKey);
                string rankKey = Constants.RankKeyPrefix + id;
                string rank = GetRank(text).ToString();

                logger.LogDebug("Rank {rank} with key {rankKey} by text id {id}", rank, rankKey, id);
                
                storage.Store(rankKey, rank);
            });
        }

        public void Run()
        {
            _subscription.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();   

            _subscription.Unsubscribe();

            _connection.Drain();
            _connection.Close();      
        }

        private double GetRank(string text)
        {
            int lettersCount = text.Count(char.IsLetter);
            
            return Math.Round(((text.Length - lettersCount) / (double)text.Length), 3);
        }

    }
}