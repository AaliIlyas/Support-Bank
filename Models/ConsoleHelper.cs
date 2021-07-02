using System;
using NLog;

namespace Support_Bank.Models
{
    public class ConsoleHelper
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public static (string name, bool statement, bool summary) GetUserResponses()
        {
            Console.Write("What is your name? ");
            var name = Console.ReadLine();
            Console.Write("Would you like a statement of all your transactions? (y/n) ");
            var statement = Console.ReadLine() == "y";
            Console.Write("Would you like total amounts that you owe and are owed? (y/n) ");
            var summary = Console.ReadLine() == "y";
            Logger.Info("Received user responses");
            return (name, statement, summary);
        }
    }
}
