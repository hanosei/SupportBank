using System;
using System.Globalization;
using System.Threading.Tasks.Dataflow;
using CsvHelper;
using Microsoft.VisualBasic;
using static System.IO.StreamReader;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SupportBank
{
    class SupportBank
    {
        public Dictionary<string, Account> _accounts = new Dictionary<string, Account>();
        public List<Transaction> transactions = new List<Transaction>();

        public void ReadCSV(string path)
        {

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.CurrentCulture);

            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var record = new Transaction
                {
                    Date = csv.GetField<DateTime>("Date"),
                    From = csv.GetField("From"),
                    To = csv.GetField("To"),
                    Narrative = csv.GetField("Narrative"),
                    Amount = csv.GetField<float>("Amount"),
                };
                transactions.Add(record);
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
            while (!nameCheck) {
                string? name = getUserInput();
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
                catch (KeyNotFoundException e)
                {
                    Console.WriteLine("Name not found in file");
                    name = getUserInput();
                }
            }        
        }

        public string? getUserInput()

        {
            Console.WriteLine("Enter a name: ");
            string? userInput = Console.ReadLine();
            return userInput;

        }
        public int getUserOptions()
        {   

            Console.WriteLine("Select from the following options \n1. Get list of all accounts \n2. Get accounts by name\nEnter 1 or 2");
            int option;
            int.TryParse(Console.ReadLine(), out option);
            return option;
        } 
    

        static void Main(string[] args)
        {
            var bank = new SupportBank();
            bank.ReadCSV("./Transactions2014.csv");
            bank.CreateAccounts();
            bool validUserOption = false;
            while (!validUserOption)
            {
                int userOption = bank.getUserOptions();
                if (userOption == 1 || userOption == 2)
                {
                    validUserOption = true;
                    if (userOption == 1)
                    {
                        bank.PrintAllAccounts();
                    }
                    else
                    {
                        bank.PrintAnAccount();
                    }
                }
               
            }
            
            
        }
    }
}