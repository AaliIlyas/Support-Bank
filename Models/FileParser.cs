using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NLog;
using System;

namespace Support_Bank.Models
{
    public class FileParser
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public static List<Transaction> GetTransactions(string path)
        {


            // if is csv , do this function
            // if is json do this function
            try
            {
                // if (Regex.IsMatch(textBox5.Text, AllowedChars))
                // {
                //     MessageBox.Show("Valid");
                // }

                // var json = @"/.json$/";
                // var csv = new Regex(@"/*.csv$/");
                // var xml = @"/.xml$/";

                // var file = File.ReadAllText(jsonPath);
                // return JsonConvert.DeserializeObject<List<Transaction>>(file);
              
                // if (Regex.IsMatch(path, json))
                // {
                //     //JsonConvert.DeserializeObject<List<Transaction>>(path);
                // }

                //var file = new XmlDocumnet();, file.load(path);
                return GetTransactionsFromCsv(path);
                    
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

        private static bool HasEmptyCell(string[] cells)
        {
            if (cells.Contains(""))
            {
                Logger.Warn($"Data incomplete. Entry has been discarded.");
                return true;
            }
            return false;
        }
        private static bool IsValidTransaction(string[] cells)
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

// using (Converter converter = new Converter(@"path/sample.json"))
// {
//     SpreadsheetConvertOptions options = new SpreadsheetConvertOptions()
//     {
//         Format = SpreadsheetFileType.Csv
//     };

//     converter.Convert(@"path/JsonToCSV.csv", options);
// }
