using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Support_Bank.Models.Finance;

namespace Support_Bank.Models
{
    internal class Person
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public string Name { get; }
        public List<Transaction> Transactions { get; }
        public Dictionary<string, double> DebtsAndCreditsReport { get; }
        public double Credit { get; private set; }
        public double Debt { get; private set; }
        public Person(string name, List<Transaction> transactions)
        {
                
            Name = name;
 
            var filteredTransactions = transactions.Where(transaction => transaction.FromAccount == Name || transaction.ToAccount == Name).ToList();

            Transactions = filteredTransactions;

            DebtsAndCreditsReport = new Dictionary<string, double>();

            foreach (var transaction in Transactions)
            {
                var isFromThisPerson = transaction.FromAccount == Name;
                var otherPerson = !isFromThisPerson ? transaction.FromAccount : transaction.ToAccount;
                if (!DebtsAndCreditsReport.ContainsKey(otherPerson))
                {
                    if (isFromThisPerson)
                    {
                        DebtsAndCreditsReport.Add(otherPerson, transaction.Amount * -1);
                    }
                    else
                    {
                        DebtsAndCreditsReport.Add(otherPerson, transaction.Amount);
                    }
                }
                else
                {
                    if (isFromThisPerson)
                    {
                        DebtsAndCreditsReport[otherPerson] -= transaction.Amount;
                    }
                    else
                    {
                        DebtsAndCreditsReport[otherPerson] += transaction.Amount;
                    }
                }
            }

            CalculateTotals(); 
        }

        private void CalculateTotals()
        {
            foreach (var entry in DebtsAndCreditsReport)
            {
                if (entry.Value >= 0)
                {
                    Credit += entry.Value;
                }

                if (entry.Value <= 0)
                {
                    Debt += Math.Abs(entry.Value);
                }
            }
        }
    }
}
