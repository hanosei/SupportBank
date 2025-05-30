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

        public void ImportFile(string filename)
        {
            string extension = Path.GetExtension(filename);
            if (filename.EndsWith(".csv"))
            {
                ReadTransactionsFromCSV(filename);
            }
            else if (filename.EndsWith(".json"))
            {
                ReadTransactionsFromJSON(filename);
            }
            else
            {
                Console.WriteLine("Unsupported file type.");
            }
        }

        public void ReadTransactionsFromJSON(string path)
        {
            string? methodName = MethodBase.GetCurrentMethod()?.Name;
            try
            {
                Logger.Info("Starting to read the JSON file...");
                List<dynamic> data = FileProcessor.ReadJSON(path);
                if (data != null)
                {

                    float amount = 0;
                    DateTime date = new DateTime();

                    foreach (var item in data)
                    {
                        if (item.GetProperty("Amount").TryGetSingle(out amount) &&
                        DateTime.TryParse(item.GetProperty("Date").GetString(), out date) &&
                        !(string.IsNullOrEmpty(item.GetProperty("ToAccount").GetString()))
                                 && !(string.IsNullOrEmpty(item.GetProperty("FromAccount").GetString()))
                                 && !(string.IsNullOrEmpty(item.GetProperty("Narrative").GetString())))
                        {
                            var record = new Transaction
                            {
                                Date = date,
                                From = item.GetProperty("FromAccount").GetString(),
                                To = item.GetProperty("ToAccount").GetString(),
                                Narrative = item.GetProperty("Narrative").GetString(),
                                Amount = amount,
                            };
                            transactions.Add(record);

                        }
                        else
                        {
                            Logger.Error("Date: " + item.GetProperty("Date").GetString() + " From: " + item.GetProperty("FromAccount").GetString()
                            + " To: " + item.GetProperty("ToAccount").GetString() + " Narrative: " + item.GetProperty("Narrative").GetString() + " Amount: " + item.GetProperty("Amount").GetSingle());
                        }
                    Logger.Info("JSON File has been read...");    
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(methodName + " " + e);
            }


        }

        public void ReadTransactionsFromCSV(string path)
        {
            string? methodName = MethodBase.GetCurrentMethod()?.Name;
            try
            {
                var csv = FileProcessor.ReadCSV(path);
                if (csv != null)
                {
                    csv.Read();
                    Logger.Info("Starting to read the CSV file...");
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        DateTime date;
                        float amount;

                        if (DateTime.TryParse(csv.GetField("Date"), out date)
                         && (float.TryParse(csv.GetField("Amount"), out amount))
                         && !(string.IsNullOrEmpty(csv.GetField("To")))
                         && !(string.IsNullOrEmpty(csv.GetField("From")))
                         && !(string.IsNullOrEmpty(csv.GetField("Narrative"))))
                        {
                            var record = new Transaction
                            {
                                Date = date,
                                From = csv.GetField("From"),
                                To = csv.GetField("To"),
                                Narrative = csv.GetField("Narrative"),
                                Amount = amount,
                            };
                            transactions.Add(record);
                        }
                        else
                        {
                            Logger.Error("Date: " + csv.GetField("Date") + " From: " + csv.GetField("From")
                            + " To: " + csv.GetField("To") + " Narrative: " + csv.GetField("Narrative")
                            + " Amount: " + csv.GetField("Amount"));
                        }
                    }
                    Logger.Info("CSV File has been read...");
                }
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