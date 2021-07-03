using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support_Bank.Models.Finance
{
    public class DeserializedTransaction
    {
        public string Date { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public string Narrative { get; set; }
        public double Amount { get; set; }

        public static Transaction ConvertToTransaction(DeserializedTransaction transaction)
        {
            var transactionComponents = new string[5] { transaction.Date, transaction.FromAccount, transaction.ToAccount, transaction.Narrative, transaction.Amount.ToString() };
            return new Transaction(transactionComponents);
        }
    }
}
