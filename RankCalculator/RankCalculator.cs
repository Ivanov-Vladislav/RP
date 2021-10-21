using Common;
using Storage;
using NATS.Client;
using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace RankCalculator
{
    public class RankCalculator
    {
        private readonly IConnection _connection;
        private readonly IAsyncSubscription _subscription;
        private readonly IStorage _storage;
        private readonly ILogger<RankCalculator> _logger;
        
        public RankCalculator(ILogger<RankCalculator> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
            _connection = new ConnectionFactory().CreateConnection();
            _subscription = GetSubscription();
        }

        private IAsyncSubscription GetSubscription()
        {   
            EventHandler<MsgHandlerEventArgs> msgHandler = async (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                string textKey = Constants.TextKeyPrefix + id;

                if (!_storage.IsKeyExist(textKey))
                {
                    _logger.LogWarning("Text key {textKey} doesn't exists", textKey);
                    return;
                }

                string text = _storage.Load(textKey);
                string rankKey = Constants.RankKeyPrefix + id;
                double rank = GetRank(text);

                _logger.LogDebug("Rank {rank} with key {rankKey} by text id {id}", rank.ToString(), rankKey, id);
                
                _storage.Store(rankKey, rank.ToString());

                RankMessage rankMessage = new RankMessage(id, rank);
                await SentMessageToEventLogger(rankMessage);
            };

            return _connection.SubscribeAsync("valuator.processing.rank", "rank_calculator", (msgHandler));
        }
    
        private async Task SentMessageToEventLogger(RankMessage rankMsg)
        {            
            using (IConnection c = new ConnectionFactory().CreateConnection())
            {
                var data = JsonSerializer.Serialize(rankMsg);
                c.Publish("rankCalculator.logging.rank", Encoding.UTF8.GetBytes(data));
                await Task.Delay(1000);

                c.Drain();
                c.Close();
            }
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