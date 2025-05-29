using System;
using System.Globalization;
using System.Threading.Tasks.Dataflow;
using CsvHelper;
using Microsoft.VisualBasic;
using static System.IO.StreamReader;
using static System.Runtime.InteropServices.JavaScript.JSType;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Fluent;
using System.Reflection;

namespace SupportBank
{
    class SupportBank
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public Dictionary<string, Account> _accounts = new Dictionary<string, Account>();
        public List<Transaction> transactions = new List<Transaction>();

        public void ReadCSV(string path)
        {
            string? methodName = MethodBase.GetCurrentMethod()?.Name;

            try
            {

                using var reader = new StreamReader(path);
                using var csv = new CsvReader(reader, CultureInfo.CurrentCulture);

                csv.Read();
                Logger.Info("Starting to read the file...");
                csv.ReadHeader();
                while (csv.Read())
                {
                    DateTime dateTemp;
                    float amountTemp;

                    if (DateTime.TryParse(csv.GetField("Date"), out dateTemp) && (float.TryParse(csv.GetField("Amount"), out amountTemp)) && !(string.IsNullOrEmpty(csv.GetField("To"))) && !(string.IsNullOrEmpty(csv.GetField("From"))) && !(string.IsNullOrEmpty(csv.GetField("Narrative"))))
                    {
                        var record = new Transaction
                        {
                            Date = dateTemp,
                            From = csv.GetField("From"),
                            To = csv.GetField("To"),
                            Narrative = csv.GetField("Narrative"),
                            Amount = amountTemp,
                        };
                        transactions.Add(record);
                    }
                    else
                    {
                        Logger.Error("Date: " + csv.GetField("Date") + " From: " + csv.GetField("From") + " To: " + csv.GetField("To") + " Narrative: " + csv.GetField("Narrative") + " Amount: " + csv.GetField("Amount"));

                    }


                }
                Logger.Info("File has been read...");
            }
            catch (Exception e)
            {
                Logger.Error(methodName + " " + e);
            }

        }
        public void CreateAccounts()
        {
            foreach (var record in transactions)
            {
                Account account;
                if (!_accounts.ContainsKey(record.From!))
                {
                    account = new Account(record.From!);
                    _accounts.Add(record.From!, account);
                }
                if (!_accounts.ContainsKey(record.To!))
                {
                    account = new Account(record.To!);
                    _accounts.Add(record.To!, account);
                }
                if (_accounts.ContainsKey(record.From!))
                {
                    account = _accounts[record.From!];
                    account.updateIsOwed(record.Amount);
                    account.UpdateTransaction(record);
                }
                if (_accounts.ContainsKey(record.To!))
                {
                    account = _accounts[record.To!];
                    account.updateOwed(record.Amount);
                    account.UpdateTransaction(record);
                }

            }
        }

        public void PrintAllAccounts()
        {
            foreach (var account in _accounts.Values)
            {
                Console.WriteLine($"{account.Name} owes {account.Owed}, and is owed {account.IsOwed} The total {account.TotalBalance}");
            }
        }

        public void PrintAnAccount()
        {
            bool nameCheck = false;
            while (!nameCheck)
            {
                string? name = UserInput.getUserInput();
                try
                {
                    Account account = _accounts[name!];
                    nameCheck = true;
                    Console.WriteLine($"Account: {name}");
                    foreach (var record in account.AccountTransactions!)
                    {
                        Console.WriteLine($"Transaction: {record.Date}, {record.Narrative}, {record.From}, {record.To}, {record.Amount}");
                    }
                }
                catch (KeyNotFoundException)
                {
                    Logger.Error("Name not found in the file");
                    Console.WriteLine("Name not found in file");
                }
            }
        }




    }
}