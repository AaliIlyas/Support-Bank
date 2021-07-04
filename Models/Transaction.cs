using System;

namespace Support_Bank.Models.Finance
{
    public class Transaction
    {
        public string Date { get; }
        public string FromAccount { get; }
        public string ToAccount { get; }
        public string Narrative { get; }
        public double Amount { get; }

        public Transaction(string[] cells)
        {
            Date = cells[0];
            FromAccount = cells[1];
            ToAccount = cells[2];
            Narrative = cells[3];
            Amount = double.Parse(cells[4]);
        }
    }
}