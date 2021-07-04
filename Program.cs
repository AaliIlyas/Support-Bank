using System.Collections.Generic;
using Support_Bank.Models;
using System;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.IO;
using Support_Bank.Models.Finance;

namespace SupportBank
{
    internal class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private static void Main(string[] args)
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget
            {
                FileName = "../../../Logs/SupportBank.log",
                Layout = @"${longdate} ${level} - ${logger}: ${message}"
            };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;

            Logger.Debug($"Parsing started {DateTime.Now.ToString("h:mm:ss tt")}.");
            var transactions = FileParser.GetTransactions("./support-bank-resources/Transactions2012.xml");
            Logger.Debug($"Parsing ended {DateTime.Now.ToString("h:mm:ss tt")}");

            GetTotalCreditsAndDebits(transactions);
                 
            var (name, statement, summary) = ConsoleHelper.GetUserResponses();

            var person = new Person(name, transactions);

            if (statement)
            {
                Logger.Info("User has opted to see their own Statement. Printing to console.");
                foreach (var transaction in person.Transactions)
                {
                    Console.WriteLine($"Date: {transaction.Date}, from: {transaction.FromAccount}, to: {transaction.ToAccount}, reason: {transaction.Narrative}, amount: {transaction.Amount.ToString("C")}");
                }
            }

            if (summary)
            {
                Logger.Info("User has opted to see their own Debits and Credits. Printing to console.");
                if (statement)
                {
                    Console.WriteLine("----------------------------------");
                }
                foreach (var entry in person.DebitsAndCreditsReport)
                {
                    Console.WriteLine(entry.Key + ": " + entry.Value.ToString("C"));
                }
            }
            Logger.Info($"Program ending {DateTime.Now.ToString("h:mm:ss tt")}");
        }

        private static void GetTotalCreditsAndDebits(List<Transaction> transactions)
        {
            Logger.Info($"In {System.Reflection.MethodBase.GetCurrentMethod().Name}! It's {DateTime.Now.ToString("h:mm:ss tt")}.");
            Console.Write("Would you like the totals of what everyone owes or is owed? (y/n) ");
            var totalOwes = Console.ReadLine() == "y";

            if (totalOwes)
            Logger.Info("User has opted to see everyone's Debits and Credits. Printing to console.");
            {
                var allNames = new List<string>();

                foreach (var transaction in transactions)
                {
                    if (!allNames.Contains(transaction.FromAccount))
                    {
                        allNames.Add(transaction.FromAccount);
                    }

                    if (!allNames.Contains(transaction.ToAccount))
                    {
                        allNames.Add(transaction.ToAccount);
                    }
                }

                foreach (var personName in allNames)
                {
                    var p = new Person(personName, transactions);
                    Console.WriteLine(p.Name + ": owes " + p.Credit.ToString("C") + ", is owed: " + p.Debt.ToString("C"));
                }
            }
            Logger.Info($"Leaving {System.Reflection.MethodBase.GetCurrentMethod().Name}! It's {DateTime.Now.ToString("h:mm:ss tt")}");
        }
    }
}
