using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Support_Bank.Models
{
    internal class Person
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public string Name { get; }
        public List<Transaction> Transactions { get; }
        public Dictionary<string, double> DebitsAndCreditsReport { get; }
        public double Credit { get; private set; }
        public double Debt { get; private set; }
        public Person(string name, List<Transaction> transactions)
        {
                
            Name = name;
 
            var filteredTransactions = transactions.Where(transaction => transaction.From == Name || transaction.To == Name).ToList();

            Transactions = filteredTransactions;

            DebitsAndCreditsReport = new Dictionary<string, double>();

            foreach (var transaction in Transactions)
            {
                var isFromThisPerson = transaction.From == Name;
                var otherPerson = !isFromThisPerson ? transaction.From : transaction.To;
                if (!DebitsAndCreditsReport.ContainsKey(otherPerson))
                {
                    if (isFromThisPerson)
                    {
                        DebitsAndCreditsReport.Add(otherPerson, transaction.Amount * -1);
                    }
                    else
                    {
                        DebitsAndCreditsReport.Add(otherPerson, transaction.Amount);
                    }
                }
                else
                {
                    if (isFromThisPerson)
                    {
                        DebitsAndCreditsReport[otherPerson] -= transaction.Amount;
                    }
                    else
                    {
                        DebitsAndCreditsReport[otherPerson] += transaction.Amount;
                    }
                }
            }

            CalculateTotals(); 
        }

        private void CalculateTotals()
        {
            foreach (var transaction in Transactions)
            {
                if (Name == transaction.From)
                {
                    Credit += transaction.Amount;
                }

                if (Name == transaction.To)
                {
                    Debt += transaction.Amount;
                }
            }
        }
    }
}
