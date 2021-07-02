using System.Collections.Generic;
using Support_Bank.Models;
using System;
using NLog;
using NLog.Config;
using NLog.Targets;

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
            var transactions = FileParser.GetTransactions("./support-bank-resources/DodgyTransactions2015.csv");

            GetTotalCreditsAndDebits();

            var (name, statement, summary) = getUserResponses.PromptUser();

            var person = new Person(name, transactions);

            if (statement)
            {
                foreach (var t in person.Transactions)
                {
                    Console.WriteLine($"Date: {t.Date}, from: {t.From}, to: {t.To}, reason: {t.Narrative}, amount: {t.Amount.ToString("C")}");
                }
            }

            if (summary)
            {
                if (statement)
                {
                    Console.WriteLine("----------------------------------");
                }
                foreach (var entry in person.DebitsAndCreditsReport)
                {
                    Console.WriteLine(entry.Key + ": " + entry.Value.ToString("C"));
                }
            }

            void GetTotalCreditsAndDebits()
            {
                Console.Write("Would you like the totals of what everyone owes or is owed? (y/n) ");
                var totalOwes = Console.ReadLine() == "y";

                if (totalOwes)
                {
                    var allNames = new List<string>();

                    foreach (var transaction in transactions)
                    {
                        if (!allNames.Contains(transaction.From))
                        {
                            allNames.Add(transaction.From);
                        }

                        if (!allNames.Contains(transaction.To))
                        {
                            allNames.Add(transaction.To);
                        }
                    }

                    foreach (var personName in allNames)
                    {
                        var p = new Person(personName, transactions);
                        Console.WriteLine(p.Name + ": owes " + p.Credit.ToString("C") + ", is owed: " + p.Debt.ToString("C"));
                    }
                }
            }
        }
    }
}
