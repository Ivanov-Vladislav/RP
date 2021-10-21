using Common;
using NATS.Client;
using System;
using System.Text;
using System.Linq;
using System.Text.Json;

namespace EventsLogger
{
    public class EventsLogger
    {
        private readonly IConnection _connection;

        private readonly IAsyncSubscription _subscription;

        public EventsLogger()
        {
            _connection = new ConnectionFactory().CreateConnection();

            _subscription = _connection.SubscribeAsync("valuator.logging.similarity", (similarityMsgHandler));         
            _subscription = _connection.SubscribeAsync("rankCalculator.logging.rank", (rankMsgHandler));
        }

        private EventHandler<MsgHandlerEventArgs> similarityMsgHandler = (sender, args) =>
        {
            var similarityMsg = JsonSerializer.Deserialize<SimilarityMessage>(args.Message.Data);
            Console.WriteLine($"Subject: {args.Message.Subject}; Text with id {similarityMsg.Id} has similarity {similarityMsg.Similarity}");
        };

        private EventHandler<MsgHandlerEventArgs> rankMsgHandler = (sender, args) =>
        {
            var rankMsg = JsonSerializer.Deserialize<RankMessage>(args.Message.Data);
            Console.WriteLine($"Subject: {args.Message.Subject}; Text with id {rankMsg.Id} has rank {rankMsg.Rank}");
        };

        public void Run()
        {
            _subscription.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();   

            _subscription.Unsubscribe();

            _connection.Drain();
            _connection.Close();  
        }
    }
}