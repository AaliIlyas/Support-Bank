﻿using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NLog;
using System;
using Newtonsoft.Json;
using Support_Bank.Models.Finance;
using System.Xml;

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
                    else
                    {
                        throw new ArgumentOutOfRangeException("Cannot find file");
                    }
                }

                if (json.IsMatch(path))
                {
                    if (File.Exists(path))
                    {
                        return GetTransactionsFromJson(path);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Cannot find file");
                    }
                }

                if (xml.IsMatch(path))
                {
                    if (File.Exists(path))
                    {
                        return GetTransactionsFromsXML(path);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Cannot find file");
                    }
                }

                Logger.Fatal("Invalid Extension");
                throw new ArgumentOutOfRangeException("File extension is not accepted, only .csv, .json and .xml are valid. No data obtained.");
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

        private static List<Transaction> GetTransactionsFromJson(string path)
        {
            var file = File.ReadAllText(path);
            var transactions = JsonConvert.DeserializeObject<List<DeserializedTransaction>>(file)
                .Where(transaction => IsValidDeserializedTransaction(transaction))
                .Select(transaction => DeserializedTransaction.ConvertToTransaction(transaction))
                .ToList();

            return transactions;
        }

        private static List<Transaction> GetTransactionsFromsXML(string path)
        {
            var file = new XmlDocument();
            file.Load(path);
            var nodes = file.SelectNodes("//SupportTransaction");

            var transactions = new List<Transaction> { };
            foreach (XmlNode node in nodes)
            {
                var serialDate = node.Attributes["Date"].Value;

                var Date = DateTime.FromOADate(int.Parse(serialDate)).ToString("dd/MM/yyyy");
                var Narrative = node.ChildNodes[0].InnerText;
                var Amount = node.ChildNodes[1].InnerText;
                var From = node.ChildNodes[2].ChildNodes[0].InnerText;
                var To = node.ChildNodes[2].ChildNodes[1].InnerText;

                var XmlProperties = new string[5] { Date, From, To, Narrative, Amount };
                var transaction = new Transaction(XmlProperties);
                transactions.Add(transaction);
            }
            return transactions;
        }

        private static bool IsValidDeserializedTransaction(DeserializedTransaction transaction)
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