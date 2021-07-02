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
        public static List<Transaction> GetTransactions(string route)
        {
            try
            {
                var path = route;
                var lines = File.ReadAllLines(path);
                lines = lines.Skip(1).ToArray();

                return lines
                    .Where(line => IsValidTransaction(line.Split(",")))
                    .Select(line => new Transaction(line.Split(",")))
                    .ToList();
            }
            catch (DirectoryNotFoundException e)
            {
                Logger.Warn("Cannot find directory: " + e);
                throw e;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Warn("Cannot find path: " + e);
                throw e;
            }
        }

        private static bool IsValidTransaction(string[] cells)
        {
            if (!double.TryParse(cells[4], out var amount) || !(amount > 0))
            {
                Logger.Warn($"'{cells[4]}' is not a valid amount. On date '{cells[0]}', from '{cells[1]}' to '{cells[2]}'. Entry has been discarded.");
                return false;
            }
            if (!DateTime.TryParse(cells[0], out var date))
            {
                Logger.Warn($"'{cells[0]}' is not a valid Date. Transaction from '{cells[1]}' to '{cells[2]}' with amount '{cells[4]}'. Entry has been discarded.");
                return false;
            }
            return true;
        }

        public static bool IsDate(Object obj)
        {
            string strDate = obj.ToString();
            try
            {
                DateTime dt = DateTime.Parse(strDate);
                if ((dt.Month != System.DateTime.Now.Month) || (dt.Day < 1 && dt.Day > 31) || dt.Year != System.DateTime.Now.Year)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
