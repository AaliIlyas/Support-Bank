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
        public Dictionary<string, double> debtsAndCreditsReport { get; }
        public double Credit { get; private set; }
        public double Debt { get; private set; }
        public Person(string name, List<Transaction> transactions)
        {
                
            Name = name;
 
            var filteredTransactions = transactions.Where(transaction => transaction.FromAccount == Name || transaction.ToAccount == Name).ToList();

            Transactions = filteredTransactions;

            debtsAndCreditsReport = new Dictionary<string, double>();

            foreach (var transaction in Transactions)
            {
                var isFromThisPerson = transaction.FromAccount == Name;
                var otherPerson = !isFromThisPerson ? transaction.FromAccount : transaction.ToAccount;
                if (!debtsAndCreditsReport.ContainsKey(otherPerson))
                {
                    if (isFromThisPerson)
                    {
                        debtsAndCreditsReport.Add(otherPerson, transaction.Amount * -1);
                    }
                    else
                    {
                        debtsAndCreditsReport.Add(otherPerson, transaction.Amount);
                    }
                }
                else
                {
                    if (isFromThisPerson)
                    {
                        debtsAndCreditsReport[otherPerson] -= transaction.Amount;
                    }
                    else
                    {
                        debtsAndCreditsReport[otherPerson] += transaction.Amount;
                    }
                }
            }

            CalculateTotals(); 
        }

        private void CalculateTotals()
        {
            foreach (var transaction in Transactions)
            {
                if (Name == transaction.FromAccount)
                {
                    Credit += transaction.Amount;
                }

                if (Name == transaction.ToAccount)
                {
                    Debt += transaction.Amount;
                }
            }
        }
    }
}
