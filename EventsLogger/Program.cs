using System;

namespace EventsLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            var eventsLogger = new EventsLogger();
            eventsLogger.Run();
        }
    }
}
