using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NLog;
using System;
using Newtonsoft.Json;
using Support_Bank.Models.Finance;

namespace Support_Bank.Models
{
    public class FileParser
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public static List<Transaction> GetTransactions(string path)
        {
            try
            {
                var json = new Regex(@"\.json$");
                var csv = new Regex(@"\.csv$");
                var xml = new Regex(@"\.xml$");

                if (csv.IsMatch(path))
                {
                    if (File.Exists(path))
                    {
                        return GetTransactionsFromCsv(path);
                    }
                }

                if (json.IsMatch(path))
                {
                    if (File.Exists(path))
                    {
                        GetTransactionsFromJson(path);
                    }
                }

                Logger.Debug("Successfully read file");
            }
            catch (DirectoryNotFoundException e)
            {
                Logger.Fatal("Cannot find directory: " + e);
                throw;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Fatal("Cannot find path: " + e);
                throw;
            }
            return null;
        }

        private static List<Transaction> GetTransactionsFromJson(string path)
        {
            var file = File.ReadAllText(path);
            var transactions = JsonConvert.DeserializeObject<List<DeserializedTransaction>>(file)
                .Where(transaction => DeserialisedDataChecks(transaction))
                .Select(transaction => DeserializedTransaction.ConvertToTransaction(transaction))
                .ToList();

            return transactions;
        }

        private static List<Transaction> GetTransactionsFromCsv(string path)
        {
            var lines = File.ReadAllLines(path);
            return lines
                .Where(line => !HasEmptyCell(line.Split(",")))
                .Where(line => IsValidTransaction(line.Split(",")))
                .Select(line => new Transaction(line.Split(",")))
                .ToList();
        }

        private static bool DeserialisedDataChecks(DeserializedTransaction transaction)
        {
            var transactionArray = new string[5] { transaction.Date, transaction.FromAccount, transaction.ToAccount, transaction.Narrative, transaction.Amount.ToString() };
            if (!HasEmptyCell(transactionArray) && IsValidTransaction(transactionArray))
            {
                return true;
            }
            return false;
        }

        public static bool HasEmptyCell(string[] cells)
        {
            if (cells.Contains(""))
            {
                Logger.Warn($"Data incomplete. Entry has been discarded.");
                return true;
            }
            return false;
        }
        public static bool IsValidTransaction(string[] cells)
        {
            if (!double.TryParse(cells[4], out var amount) || !(amount >= 0))
            {
                Logger.Warn($"'{cells[4]}' is not a valid amount. On date '{cells[4]}', from '{cells[1]}' to '{cells[2]}'. Entry has been discarded.");
                return false;
            }
            if (!DateTime.TryParse(cells[0], out _))
            {
                Logger.Warn($"'{cells[0]}' is not a valid Date. Transaction from '{cells[1]}' to '{cells[2]}' with amount '{cells[4]}'. Entry has been discarded.");
                return false;
            }
            return true;
        }
    }
}